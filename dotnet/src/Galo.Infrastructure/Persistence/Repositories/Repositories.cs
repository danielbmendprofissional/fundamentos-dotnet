using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Competicoes.Interfaces;
using Galo.Domain.Elenco.Entities;
using Galo.Domain.Elenco.Interfaces;
using Galo.Domain.Ingressos.Entities;
using Galo.Domain.Ingressos.Interfaces;
using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.Interfaces;
using Galo.Domain.Torcida.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence.Repositories
{
    // REPOSITÓRIOS (lado de escrita): carregam o AGREGADO COMPLETO, com tracking, para o domínio alterar.
    //
    // "Atualizar" é intencionalmente vazio: a entidade já está rastreada pelo ChangeTracker (Unit of Work), então o
    // SaveChanges detecta sozinho o que mudou. Chamar context.Update() num grafo faria o EF marcar filhos NOVOS como
    // Modified (=> UPDATE de linha inexistente => erro de concorrência). O método existe para tornar a intenção explícita.

    internal sealed class JogadorRepository : IJogadorRepository
    {
        private readonly GaloDbContext _context;
        public JogadorRepository(GaloDbContext context) => _context = context;

        public Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Jogadores.Include(j => j.Contratos).FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

        public async Task AdicionarAsync(Jogador jogador, CancellationToken cancellationToken = default)
            => await _context.Jogadores.AddAsync(jogador, cancellationToken);

        public void Atualizar(Jogador jogador) { }
    }

    internal sealed class ClubeRepository : IClubeRepository
    {
        private readonly GaloDbContext _context;
        public ClubeRepository(GaloDbContext context) => _context = context;

        public Task<Clube?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Clubes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task AdicionarAsync(Clube clube, CancellationToken cancellationToken = default)
            => await _context.Clubes.AddAsync(clube, cancellationToken);
    }

    internal sealed class EstadioRepository : IEstadioRepository
    {
        private readonly GaloDbContext _context;
        public EstadioRepository(GaloDbContext context) => _context = context;

        public Task<Estadio?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Estadios.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public async Task AdicionarAsync(Estadio estadio, CancellationToken cancellationToken = default)
            => await _context.Estadios.AddAsync(estadio, cancellationToken);
    }

    internal sealed class CampeonatoRepository : ICampeonatoRepository
    {
        private readonly GaloDbContext _context;
        public CampeonatoRepository(GaloDbContext context) => _context = context;

        public Task<Campeonato?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Campeonatos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task AdicionarAsync(Campeonato campeonato, CancellationToken cancellationToken = default)
            => await _context.Campeonatos.AddAsync(campeonato, cancellationToken);
    }

    internal sealed class PartidaRepository : IPartidaRepository
    {
        private readonly GaloDbContext _context;
        public PartidaRepository(GaloDbContext context) => _context = context;

        // Dois Includes de COLEÇÃO no mesmo SELECT geram produto cartesiano (gols x setores).
        // AsSplitQuery() troca por 3 SELECTs simples. Compare no laboratório: /api/v1/estudos/ef/projecao.
        public Task<Partida?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Partidas
                .Include(p => p.Gols)
                .Include(p => p.Setores)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task AdicionarAsync(Partida partida, CancellationToken cancellationToken = default)
            => await _context.Partidas.AddAsync(partida, cancellationToken);

        public void Atualizar(Partida partida) { }
    }

    internal sealed class TorcedorRepository : ITorcedorRepository
    {
        private readonly GaloDbContext _context;
        public TorcedorRepository(GaloDbContext context) => _context = context;

        public Task<Torcedor?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Torcedores.Include(t => t.Enderecos).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public Task<bool> ExistePorCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
            => _context.Torcedores.AnyAsync(t => t.Cpf == cpf, cancellationToken);

        public Task<bool> ExistePorEmailAsync(Email email, CancellationToken cancellationToken = default)
            => _context.Torcedores.AnyAsync(t => t.Email == email, cancellationToken);

        public async Task AdicionarAsync(Torcedor torcedor, CancellationToken cancellationToken = default)
            => await _context.Torcedores.AddAsync(torcedor, cancellationToken);

        public void Atualizar(Torcedor torcedor) { }
    }

    internal sealed class CompraIngressoRepository : ICompraIngressoRepository
    {
        private readonly GaloDbContext _context;
        public CompraIngressoRepository(GaloDbContext context) => _context = context;

        public Task<CompraIngresso?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Compras
                .Include(c => c.Itens)
                .Include(c => c.Pagamentos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task AdicionarAsync(CompraIngresso compra, CancellationToken cancellationToken = default)
            => await _context.Compras.AddAsync(compra, cancellationToken);

        public void Atualizar(CompraIngresso compra) { }
    }
}
