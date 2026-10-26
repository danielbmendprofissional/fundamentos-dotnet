using System.Diagnostics;
using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Elenco.Entities;
using Galo.Domain.Elenco.Enums;
using Galo.Domain.Elenco.ValueObjects;
using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.Enums;
using Galo.Domain.Torcida.ValueObjects;
using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Galo.Infrastructure.Seed
{
    public sealed record SeedOptions(int Jogadores = 60, int Torcedores = 2000, int PartidasEncerradas = 400, int PartidasAgendadas = 12);

    public sealed record SeedResultado(
        int Clubes, int Estadios, int Campeonatos, int Jogadores, int Torcedores, int PartidasEncerradas, int PartidasAgendadas, long DuracaoMs);

    /// <summary>
    /// Gera dados de EXEMPLO (nomes e números fictícios; clubes/estádios reais só para dar contexto) em volume
    /// suficiente para os experimentos do laboratório terem diferença mensurável.
    ///
    /// Técnicas de carga em massa usadas aqui (para estudar):
    ///  - AutoDetectChanges desligado (o EF não reanalisa o grafo a cada Add);
    ///  - AddRange + SaveChanges em LOTES (batching de INSERTs do provider) + ChangeTracker.Clear() entre lotes
    ///    (senão o tracker cresce sem parar e tudo fica lento);
    ///  - Eventos de domínio DESCARTADOS (não queremos disparar 2000 e-mails de boas-vindas no seed).
    /// </summary>
    public sealed class DadosIniciaisSeeder
    {
        private const int TamanhoLote = 500;

        private readonly GaloDbContext _db;
        private readonly ILogger<DadosIniciaisSeeder> _logger;

        public DadosIniciaisSeeder(GaloDbContext db, ILogger<DadosIniciaisSeeder> logger)
        {
            _db = db;
            _logger = logger;
        }

        private static readonly string[] Nomes =
        {
            "Lucas", "Mateus", "Gabriel", "Pedro", "Rafael", "Bruno", "Thiago", "Felipe", "Gustavo", "Leonardo", "Diego", "Rodrigo",
            "Marcelo", "Vinicius", "Eduardo", "Henrique", "Caio", "Davi", "Arthur", "Samuel", "Igor", "Renan", "Otávio", "Murilo",
            "Ana", "Maria", "Juliana", "Camila", "Fernanda", "Beatriz", "Larissa", "Patrícia", "Aline", "Carolina", "Mariana", "Letícia"
        };

        private static readonly string[] Sobrenomes =
        {
            "Silva", "Souza", "Oliveira", "Santos", "Pereira", "Lima", "Carvalho", "Ferreira", "Almeida", "Ribeiro", "Gomes", "Martins",
            "Rocha", "Barbosa", "Cardoso", "Teixeira", "Moreira", "Nunes", "Mendes", "Araújo", "Dias", "Castro", "Campos", "Freitas"
        };

        private static readonly (string Nome, string Sigla, string Cidade, string Uf, int Ano)[] Clubes =
        {
            ("Clube Atlético Mineiro", "CAM", "Belo Horizonte", "MG", 1908),
            ("Cruzeiro", "CRU", "Belo Horizonte", "MG", 1921),
            ("América-MG", "AME", "Belo Horizonte", "MG", 1912),
            ("Flamengo", "FLA", "Rio de Janeiro", "RJ", 1895),
            ("Palmeiras", "PAL", "São Paulo", "SP", 1914),
            ("Corinthians", "COR", "São Paulo", "SP", 1910),
            ("São Paulo", "SAO", "São Paulo", "SP", 1930),
            ("Santos", "SAN", "Santos", "SP", 1912),
            ("Fluminense", "FLU", "Rio de Janeiro", "RJ", 1902),
            ("Botafogo", "BOT", "Rio de Janeiro", "RJ", 1904),
            ("Vasco da Gama", "VAS", "Rio de Janeiro", "RJ", 1898),
            ("Grêmio", "GRE", "Porto Alegre", "RS", 1903),
            ("Internacional", "INT", "Porto Alegre", "RS", 1909),
            ("Bahia", "BAH", "Salvador", "BA", 1931),
            ("Vitória", "VIT", "Salvador", "BA", 1899),
            ("Fortaleza", "FOR", "Fortaleza", "CE", 1918),
            ("Athletico Paranaense", "CAP", "Curitiba", "PR", 1924),
            ("Coritiba", "CFC", "Curitiba", "PR", 1909),
            ("Red Bull Bragantino", "RBB", "Bragança Paulista", "SP", 1928),
            ("Cuiabá", "CUI", "Cuiabá", "MT", 2001)
        };

        private static readonly (string Nome, string Cidade, int Capacidade)[] Estadios =
        {
            ("Mineirão", "Belo Horizonte", 61000),
            ("Arena MRV", "Belo Horizonte", 46000),
            ("Independência", "Belo Horizonte", 23000),
            ("Maracanã", "Rio de Janeiro", 78000),
            ("Allianz Parque", "São Paulo", 43000),
            ("Neo Química Arena", "São Paulo", 49000),
            ("Morumbi", "São Paulo", 66000),
            ("Beira-Rio", "Porto Alegre", 50000)
        };

        public async Task<SeedResultado> ExecutarAsync(SeedOptions opcoes, CancellationToken cancellationToken = default)
        {
            var relogio = Stopwatch.StartNew();
            var rnd = new Random(2026); // semente fixa: resultados repetíveis entre execuções
            _db.ChangeTracker.AutoDetectChangesEnabled = false;

            // ---------- 1) base (só se ainda não existir) ----------
            if (!await _db.Clubes.AnyAsync(cancellationToken))
            {
                _db.Clubes.AddRange(Clubes.Select(c => Clube.Criar(c.Nome, c.Sigla, c.Cidade, c.Uf, c.Ano, ehOGalo: c.Sigla == "CAM")));
                _db.Estadios.AddRange(Estadios.Select(e => Estadio.Criar(e.Nome, e.Cidade, e.Capacidade)));

                var ano = DateTime.UtcNow.Year;
                _db.Campeonatos.AddRange(
                    Campeonato.Criar($"Campeonato Brasileiro {ano}", ano, TipoCampeonato.Brasileirao),
                    Campeonato.Criar($"Copa do Brasil {ano}", ano, TipoCampeonato.CopaDoBrasil),
                    Campeonato.Criar($"Campeonato Mineiro {ano}", ano, TipoCampeonato.Mineiro));
                await SalvarAsync(cancellationToken);
            }

            var clubes = await _db.Clubes.AsNoTracking().ToListAsync(cancellationToken);
            var estadios = await _db.Estadios.AsNoTracking().ToListAsync(cancellationToken);
            var campeonatos = await _db.Campeonatos.AsNoTracking().ToListAsync(cancellationToken);
            var galo = clubes.Single(c => c.EhOGalo);
            var adversarios = clubes.Where(c => !c.EhOGalo).ToList();
            var brasileirao = campeonatos.First(c => c.Tipo == TipoCampeonato.Brasileirao);

            // ---------- 2) jogadores ----------
            var jogadores = new List<Jogador>();
            var posicoes = Enum.GetValues<Posicao>();
            for (var i = 0; i < opcoes.Jogadores; i++)
            {
                var nascimento = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-rnd.Next(18, 38)).AddDays(-rnd.Next(0, 365)));
                var j = Jogador.Criar(
                    NomeJogador.Create($"{Nomes[rnd.Next(24)]} {Sobrenomes[rnd.Next(Sobrenomes.Length)]}"),
                    apelido: rnd.Next(3) == 0 ? $"Galo {i + 1}" : null,
                    NumeroCamisa.Create(i % 99 + 1),
                    posicoes[i % posicoes.Length],
                    nascimento,
                    nacionalidade: rnd.Next(10) == 0 ? "Argentina" : "Brasil",
                    ValorMercado.Create(rnd.Next(0, 80) * 250_000m));

                var inicio = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-rnd.Next(1, 24)));
                j.RegistrarContrato(inicio, inicio.AddYears(rnd.Next(1, 5)), rnd.Next(20, 800) * 1_000m,
                    rnd.Next(2) == 0 ? rnd.Next(10, 200) * 1_000_000m : null);

                j.ClearDomainEvents();
                jogadores.Add(j);
            }
            await AdicionarEmLotesAsync(jogadores, cancellationToken);

            // ---------- 3) torcedores ----------
            var existentes = await _db.Torcedores.CountAsync(cancellationToken);
            var planos = Enum.GetValues<PlanoSocio>();
            var torcedores = new List<Torcedor>(TamanhoLote);
            for (var i = 0; i < opcoes.Torcedores; i++)
            {
                var seq = existentes + i + 1;
                var nome = $"{Nomes[rnd.Next(Nomes.Length)]} {Sobrenomes[rnd.Next(Sobrenomes.Length)]} {Sobrenomes[rnd.Next(Sobrenomes.Length)]}";
                var t = Torcedor.Cadastrar(
                    NomeCompleto.Create(nome),
                    Cpf.Create(Cpf.GerarValido(100_000_000L + seq)),   // CPF VÁLIDO porém fictício e sequencial
                    Email.Create($"torcedor{seq}@exemplo-galo.test"),
                    rnd.Next(4) == 0 ? null : Telefone.Create($"31 9{rnd.Next(1000, 9999)}-{rnd.Next(1000, 9999)}"),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-rnd.Next(14, 80)).AddDays(-rnd.Next(0, 365))));

                if (rnd.Next(10) < 7) t.AlterarPlano(planos[rnd.Next(1, planos.Length)]);
                if (rnd.Next(2) == 0)
                    t.AdicionarEndereco($"3{rnd.Next(1000000, 9999999)}", "Avenida Afonso Pena", rnd.Next(1, 3000).ToString(),
                        "Centro", "Belo Horizonte", "MG");

                t.ClearDomainEvents();
                torcedores.Add(t);

                if (torcedores.Count == TamanhoLote) { await AdicionarEmLotesAsync(torcedores, cancellationToken); torcedores.Clear(); }
            }
            if (torcedores.Count > 0) await AdicionarEmLotesAsync(torcedores, cancellationToken);

            // ---------- 4) partidas encerradas (histórico com gols) ----------
            var jogadoresDoGalo = await _db.Jogadores.AsNoTracking().Select(j => j.Id).Take(40).ToListAsync(cancellationToken);
            var partidas = new List<Partida>(TamanhoLote);
            for (var i = 0; i < opcoes.PartidasEncerradas; i++)
            {
                var comGalo = rnd.Next(100) < 40;
                var (mandante, visitante) = SortearConfronto(rnd, galo, adversarios, comGalo);
                var estadio = mandante.EhOGalo ? estadios[rnd.Next(2)] : estadios[rnd.Next(estadios.Count)];

                var p = Partida.Agendar(brasileirao.Id, mandante.Id, visitante.Id, estadio.Id,
                    DateTime.UtcNow.AddDays(-rnd.Next(1, 200)).AddHours(-rnd.Next(0, 24)), rnd.Next(1, 39));
                p.Iniciar();

                var golsM = Gols(rnd); var golsV = Gols(rnd);
                RegistrarGols(p, p.MandanteId, golsM, rnd, mandante.EhOGalo ? jogadoresDoGalo : null);
                RegistrarGols(p, p.VisitanteId, golsV, rnd, visitante.EhOGalo ? jogadoresDoGalo : null);
                p.Encerrar();

                p.ClearDomainEvents();
                partidas.Add(p);
                if (partidas.Count == TamanhoLote) { await AdicionarEmLotesAsync(partidas, cancellationToken); partidas.Clear(); }
            }
            if (partidas.Count > 0) await AdicionarEmLotesAsync(partidas, cancellationToken);

            // ---------- 5) partidas agendadas (com ingressos à venda) ----------
            var futuras = new List<Partida>();
            for (var i = 0; i < opcoes.PartidasAgendadas; i++)
            {
                var (mandante, visitante) = SortearConfronto(rnd, galo, adversarios, comGalo: true);
                var p = Partida.Agendar(brasileirao.Id, mandante.Id, visitante.Id, estadios[mandante.EhOGalo ? 0 : 1].Id,
                    DateTime.UtcNow.AddDays(3 + i * 7), 20 + i);
                p.AdicionarSetor("Norte", 5000, 80m);
                p.AdicionarSetor("Sul", 5000, 80m);
                p.AdicionarSetor("Superior", 3000, 150m);
                p.AdicionarSetor("Camarote", 200, 600m);
                p.ClearDomainEvents();
                futuras.Add(p);
            }
            await AdicionarEmLotesAsync(futuras, cancellationToken);

            _db.ChangeTracker.AutoDetectChangesEnabled = true;
            relogio.Stop();

            var resultado = new SeedResultado(clubes.Count, estadios.Count, campeonatos.Count, opcoes.Jogadores, opcoes.Torcedores,
                opcoes.PartidasEncerradas, opcoes.PartidasAgendadas, relogio.ElapsedMilliseconds);
            _logger.LogInformation("Seed concluído em {Ms} ms: {@Resultado}", relogio.ElapsedMilliseconds, resultado);
            return resultado;
        }

        /// <summary>
        /// Apaga TODOS os dados de exemplo. Usa ExecuteDeleteAsync (EF Core 7+): um DELETE em massa direto no banco,
        /// sem carregar nada na memória nem rastrear entidades. A ordem respeita as chaves estrangeiras.
        /// </summary>
        public async Task LimparAsync(CancellationToken cancellationToken = default)
        {
            await _db.Compras.ExecuteDeleteAsync(cancellationToken);      // itens e pagamentos caem em cascata
            await _db.Partidas.ExecuteDeleteAsync(cancellationToken);     // gols e setores caem em cascata
            await _db.Torcedores.ExecuteDeleteAsync(cancellationToken);   // endereços em cascata
            await _db.Jogadores.ExecuteDeleteAsync(cancellationToken);    // contratos em cascata
            await _db.Campeonatos.ExecuteDeleteAsync(cancellationToken);
            await _db.Estadios.ExecuteDeleteAsync(cancellationToken);
            await _db.Clubes.ExecuteDeleteAsync(cancellationToken);
        }

        // ---------------- helpers ----------------

        private static (Clube Mandante, Clube Visitante) SortearConfronto(Random rnd, Clube galo, List<Clube> adversarios, bool comGalo)
        {
            if (comGalo)
            {
                var adv = adversarios[rnd.Next(adversarios.Count)];
                return rnd.Next(2) == 0 ? (galo, adv) : (adv, galo);
            }

            var a = adversarios[rnd.Next(adversarios.Count)];
            Clube b;
            do { b = adversarios[rnd.Next(adversarios.Count)]; } while (b.Id == a.Id);
            return (a, b);
        }

        // distribuição simples: 0 a 4 gols, com 0 e 1 mais prováveis
        private static int Gols(Random rnd) => rnd.Next(100) switch { < 30 => 0, < 62 => 1, < 84 => 2, < 95 => 3, _ => 4 };

        private static void RegistrarGols(Partida p, Guid clubeId, int quantidade, Random rnd, List<Guid>? elenco)
        {
            var minutos = Enumerable.Range(0, quantidade).Select(_ => rnd.Next(1, 95)).OrderBy(m => m).ToList();
            foreach (var minuto in minutos)
                p.RegistrarGol(clubeId, elenco is { Count: > 0 } ? elenco[rnd.Next(elenco.Count)] : null, minuto);
        }

        private async Task AdicionarEmLotesAsync<T>(IEnumerable<T> itens, CancellationToken ct) where T : class
        {
            foreach (var lote in itens.Chunk(TamanhoLote))
            {
                _db.Set<T>().AddRange(lote);
                await SalvarAsync(ct);
            }
        }

        private async Task SalvarAsync(CancellationToken ct)
        {
            // AutoDetectChanges está off: como só ADICIONAMOS entidades novas (já rastreadas pelo AddRange), não precisa.
            await _db.SaveChangesAsync(ct);
            _db.ChangeTracker.Clear();
        }
    }
}
