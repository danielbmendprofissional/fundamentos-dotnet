using Galo.Application.Common.Models;
using Galo.Application.Competicoes.Interfaces;
using Galo.Application.Competicoes.Queries;
using Galo.Application.Elenco.Interfaces;
using Galo.Application.Elenco.Queries;
using Galo.Application.Ingressos.Interfaces;
using Galo.Application.Ingressos.Queries;
using Galo.Application.Torcida.Interfaces;
using Galo.Application.Torcida.Queries;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Elenco.Enums;
using Galo.Domain.Torcida.Enums;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence.Repositories
{
    // REPOSITÓRIOS DE LEITURA (CQRS). Receita de toda consulta aqui:
    //   1. filtrar/ordenar/paginar NO BANCO (IQueryable, nada de ToList antes do Where);
    //   2. PROJETAR direto para o DTO com Select: o EF só seleciona as colunas usadas (sem SELECT *),
    //      sem rastrear entidades e sem carregar o que a tela não mostra;
    //   3. materializar uma vez (ToListAsync / FirstOrDefaultAsync).

    internal sealed class JogadorQueryRepository : IJogadorQueryRepository
    {
        private readonly GaloQueryDbContext _db;
        public JogadorQueryRepository(GaloQueryDbContext db) => _db = db;

        public async Task<PagedResult<JogadorResumoDto>> ListarAsync(
            Posicao? posicao, StatusJogador? status, int pagina, int tamanho, CancellationToken cancellationToken)
        {
            IQueryable<Persistence.QueryModels.JogadorQM> query = _db.Jogadores;

            // O enum é guardado como texto: compara com o nome.
            if (posicao is not null) { var p = posicao.Value.ToString(); query = query.Where(j => j.Posicao == p); }
            if (status is not null) { var s = status.Value.ToString(); query = query.Where(j => j.Status == s); }

            var total = await query.CountAsync(cancellationToken);

            var itens = await query
                .OrderBy(j => j.Camisa).ThenBy(j => j.Nome)           // ordenação estável para paginar
                .Skip((pagina - 1) * tamanho).Take(tamanho)
                .Select(j => new JogadorResumoDto(j.Id, j.Nome, j.Apelido, j.Camisa, j.Posicao, j.Status, j.ValorMercado))
                .ToListAsync(cancellationToken);

            return new PagedResult<JogadorResumoDto>(itens, pagina, tamanho, total);
        }

        public async Task<JogadorDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken)
            => await _db.Jogadores
                .Where(j => j.Id == id)
                .Select(j => new JogadorDetalheDto(
                    j.Id, j.Nome, j.Apelido, j.Camisa, j.Posicao, j.Status, j.DataNascimento, j.Nacionalidade, j.ValorMercado,
                    j.Contratos.OrderBy(c => c.Inicio)
                        .Select(c => new ContratoDto(c.Id, c.Inicio, c.Fim, c.SalarioMensal, c.ClausulaRescisoria))
                        .ToList()))
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);
    }

    internal sealed class PartidaQueryRepository : IPartidaQueryRepository
    {
        private readonly GaloQueryDbContext _db;
        public PartidaQueryRepository(GaloQueryDbContext db) => _db = db;

        public async Task<PagedResult<PartidaResumoDto>> ListarAsync(
            Guid? campeonatoId, Guid? clubeId, StatusPartida? status, int pagina, int tamanho, CancellationToken cancellationToken)
        {
            IQueryable<Persistence.QueryModels.PartidaQM> query = _db.Partidas;

            if (campeonatoId is not null) query = query.Where(p => p.CampeonatoId == campeonatoId);
            if (clubeId is not null) query = query.Where(p => p.MandanteId == clubeId || p.VisitanteId == clubeId);
            if (status is not null) { var s = status.Value.ToString(); query = query.Where(p => p.Status == s); }

            var total = await query.CountAsync(cancellationToken);

            // Navegações no Select viram JOINs (campeonato, mandante, visitante) - 1 ida ao banco.
            var itens = await query
                .OrderByDescending(p => p.DataHoraUtc).ThenBy(p => p.Id)
                .Skip((pagina - 1) * tamanho).Take(tamanho)
                .Select(p => new PartidaResumoDto(
                    p.Id, p.Campeonato.Nome, p.Rodada, p.Mandante.Nome, p.Visitante.Nome,
                    p.DataHoraUtc, p.Status, p.GolsMandante, p.GolsVisitante))
                .ToListAsync(cancellationToken);

            return new PagedResult<PartidaResumoDto>(itens, pagina, tamanho, total);
        }

        public async Task<PartidaDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken)
            => await _db.Partidas
                .Where(p => p.Id == id)
                .Select(p => new PartidaDetalheDto(
                    p.Id, p.Campeonato.Nome, p.Rodada, p.Estadio.Nome, p.Mandante.Nome, p.Visitante.Nome,
                    p.DataHoraUtc, p.Status, p.GolsMandante, p.GolsVisitante,
                    p.Gols.OrderBy(g => g.Minuto)
                        .Select(g => new GolDto(g.Minuto, g.Clube.Nome, g.Jogador != null ? g.Jogador.Nome : null, g.Tipo))
                        .ToList(),
                    p.Setores.OrderBy(s => s.Nome)
                        .Select(s => new SetorDto(s.Nome, s.Capacidade, s.Disponiveis, s.Preco))
                        .ToList()))
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

        /// <summary>
        /// Classificação calculada a partir das partidas ENCERRADAS. Busca só as colunas necessárias
        /// (projeção) e agrega em memória: são poucas linhas (centenas). Para milhões de linhas, o ideal seria
        /// GROUP BY no banco ou uma view/tabela materializada - veja a comparação no laboratório de memória.
        /// </summary>
        public async Task<IReadOnlyList<ClassificacaoItemDto>> ObterClassificacaoAsync(Guid campeonatoId, CancellationToken cancellationToken)
        {
            var encerrada = StatusPartida.Encerrada.ToString();

            var jogos = await _db.Partidas
                .Where(p => p.CampeonatoId == campeonatoId && p.Status == encerrada)
                .Select(p => new { p.MandanteId, p.VisitanteId, p.GolsMandante, p.GolsVisitante })
                .ToListAsync(cancellationToken);

            var ids = jogos.SelectMany(j => new[] { j.MandanteId, j.VisitanteId }).Distinct().ToList();
            var clubes = await _db.Clubes.Where(c => ids.Contains(c.Id))
                .Select(c => new { c.Id, c.Nome, c.Sigla })
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            // cada jogo gera 2 linhas (visão do mandante e do visitante)
            var linhas = jogos.SelectMany(j => new[]
            {
                (Clube: j.MandanteId, Pro: j.GolsMandante, Contra: j.GolsVisitante),
                (Clube: j.VisitanteId, Pro: j.GolsVisitante, Contra: j.GolsMandante)
            });

            var tabela = linhas
                .GroupBy(l => l.Clube)
                .Select(g => new
                {
                    ClubeId = g.Key,
                    Jogos = g.Count(),
                    Vitorias = g.Count(l => l.Pro > l.Contra),
                    Empates = g.Count(l => l.Pro == l.Contra),
                    Derrotas = g.Count(l => l.Pro < l.Contra),
                    GolsPro = g.Sum(l => l.Pro),
                    GolsContra = g.Sum(l => l.Contra)
                })
                .Select(t => new
                {
                    t.ClubeId, t.Jogos, t.Vitorias, t.Empates, t.Derrotas, t.GolsPro, t.GolsContra,
                    Saldo = t.GolsPro - t.GolsContra,
                    Pontos = t.Vitorias * 3 + t.Empates
                })
                .OrderByDescending(t => t.Pontos).ThenByDescending(t => t.Vitorias).ThenByDescending(t => t.Saldo)
                .ThenByDescending(t => t.GolsPro)
                .ToList();

            return tabela
                .Select((t, i) => new ClassificacaoItemDto(
                    i + 1, t.ClubeId, clubes[t.ClubeId].Nome, clubes[t.ClubeId].Sigla,
                    t.Jogos, t.Vitorias, t.Empates, t.Derrotas, t.GolsPro, t.GolsContra, t.Saldo, t.Pontos))
                .ToList();
        }
    }

    internal sealed class CatalogoQueryRepository : ICatalogoQueryRepository
    {
        private readonly GaloQueryDbContext _db;
        public CatalogoQueryRepository(GaloQueryDbContext db) => _db = db;

        public async Task<IReadOnlyList<ClubeDto>> ListarClubesAsync(CancellationToken cancellationToken)
            => await _db.Clubes.OrderBy(c => c.Nome)
                .Select(c => new ClubeDto(c.Id, c.Nome, c.Sigla, c.Cidade, c.Uf, c.EhOGalo))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<EstadioDto>> ListarEstadiosAsync(CancellationToken cancellationToken)
            => await _db.Estadios.OrderBy(e => e.Nome)
                .Select(e => new EstadioDto(e.Id, e.Nome, e.Cidade, e.Capacidade))
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<CampeonatoDto>> ListarCampeonatosAsync(CancellationToken cancellationToken)
            => await _db.Campeonatos.OrderByDescending(c => c.Temporada).ThenBy(c => c.Nome)
                .Select(c => new CampeonatoDto(c.Id, c.Nome, c.Temporada, c.Tipo, c.Status))
                .ToListAsync(cancellationToken);
    }

    internal sealed class TorcedorQueryRepository : ITorcedorQueryRepository
    {
        private readonly GaloQueryDbContext _db;
        public TorcedorQueryRepository(GaloQueryDbContext db) => _db = db;

        public async Task<PagedResult<TorcedorResumoDto>> ListarAsync(PlanoSocio? plano, int pagina, int tamanho, CancellationToken cancellationToken)
        {
            IQueryable<Persistence.QueryModels.TorcedorQM> query = _db.Torcedores;
            if (plano is not null) { var p = plano.Value.ToString(); query = query.Where(t => t.Plano == p); }

            var total = await query.CountAsync(cancellationToken);

            var itens = await query
                .OrderBy(t => t.Nome).ThenBy(t => t.Id)
                .Skip((pagina - 1) * tamanho).Take(tamanho)
                .Select(t => new TorcedorResumoDto(t.Id, t.Nome, t.Email, t.Plano, t.Status))
                .ToListAsync(cancellationToken);

            return new PagedResult<TorcedorResumoDto>(itens, pagina, tamanho, total);
        }

        public async Task<TorcedorDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken)
        {
            var t = await _db.Torcedores.Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id, x.Nome, x.Cpf, x.Email, x.Telefone, x.DataNascimento, x.Plano, x.Status, x.EnderecoPrincipalId,
                    Enderecos = x.Enderecos.ToList()
                })
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

            if (t is null) return null;

            // dado pessoal sensível (LGPD): o CPF sai mascarado da API
            var cpfMascarado = t.Cpf.Length == 11 ? $"***.***.***-{t.Cpf[9..]}" : "***";

            return new TorcedorDetalheDto(t.Id, t.Nome, cpfMascarado, t.Email, t.Telefone, t.DataNascimento, t.Plano, t.Status,
                t.Enderecos.Select(e => new EnderecoDto(e.Id, e.Cep, e.Logradouro, e.Numero, e.Bairro, e.Cidade,
                    e.Estado, e.Pais, e.Complemento, e.Id == t.EnderecoPrincipalId)).ToList());
        }
    }

    internal sealed class CompraQueryRepository : ICompraQueryRepository
    {
        private readonly GaloQueryDbContext _db;
        public CompraQueryRepository(GaloQueryDbContext db) => _db = db;

        public async Task<CompraDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken)
            => await _db.Compras.Where(c => c.Id == id)
                .Select(c => new CompraDetalheDto(
                    c.Id, c.Numero, c.TorcedorId, c.PartidaId, c.Status, c.ReservaConfirmada, c.MotivoCancelamento,
                    c.ValorTotal, c.DataCriacao,
                    c.Itens.Select(i => new ItemCompraDto(i.Setor, i.Quantidade, i.PrecoUnitario, i.PorcentagemDesconto, i.ValorTotal)).ToList(),
                    c.Pagamentos.Select(p => new PagamentoDto(p.Id, p.Metodo, p.Status, p.Valor, p.CodigoTransacao, p.DataPagamento)).ToList()))
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<CompraResumoDto>> ListarDoTorcedorAsync(Guid torcedorId, int pagina, int tamanho, CancellationToken cancellationToken)
        {
            var query = _db.Compras.Where(c => c.TorcedorId == torcedorId);
            var total = await query.CountAsync(cancellationToken);

            var itens = await query
                .OrderByDescending(c => c.DataCriacao).ThenBy(c => c.Id)
                .Skip((pagina - 1) * tamanho).Take(tamanho)
                .Select(c => new CompraResumoDto(c.Id, c.Numero, c.PartidaId, c.Status, c.ValorTotal, c.DataCriacao))
                .ToListAsync(cancellationToken);

            return new PagedResult<CompraResumoDto>(itens, pagina, tamanho, total);
        }
    }
}
