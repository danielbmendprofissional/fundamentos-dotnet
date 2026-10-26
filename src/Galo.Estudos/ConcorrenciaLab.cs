using System.Diagnostics;
using Galo.Application.Ingressos.Commands;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Ingressos.Enums;
using Galo.Estudos.Common;
using Galo.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE COMPETIÇÃO (concorrência de dados): "N torcedores disputam os últimos K ingressos".
    ///
    /// O problema: LOST UPDATE / overselling.
    ///   Torcedor A lê Disponiveis = 1;  Torcedor B lê Disponiveis = 1;
    ///   A grava 0 (vendeu);             B grava 0 (vendeu)   => 2 ingressos vendidos para 1 vaga.
    ///
    /// Cada estratégia abaixo roda o MESMO cenário, com um setor novo (criado e apagado pelo experimento):
    ///   capacidade K, N compradores em paralelo, cada um querendo 1 ingresso.
    /// Resultado correto = exatamente K vendas e Disponiveis = 0.
    /// </summary>
    public sealed class ConcorrenciaLab
    {
        private const string NomeSetor = "Lab";

        private readonly IServiceScopeFactory _scopes;
        public ConcorrenciaLab(IServiceScopeFactory scopes) => _scopes = scopes;

        private sealed class Placar
        {
            public int Sucessos, Esgotados, Conflitos, Erros;
        }

        // ================================================================================================
        // 1) AS 5 ESTRATÉGIAS
        // ================================================================================================
        public async Task<ExperimentoResultado> EstrategiasAsync(int capacidade, int compradores, int grau, CancellationToken ct)
        {
            capacidade = Math.Clamp(capacidade, 1, 200);
            compradores = Math.Clamp(compradores, 2, 500);
            grau = Math.Clamp(grau, 2, 32);

            var medicoes = new List<Medicao>
            {
                await Rodar("1. INGÊNUA (ler -> decrementar -> gravar)",
                    "SELECT do estoque, decide em memória, UPDATE com o valor calculado. Sem nenhuma proteção",
                    capacidade, compradores, grau, ComprarIngenuo, ct),

                await Rodar("2. LOCK EM PROCESSO (SemaphoreSlim)",
                    "A mesma lógica ingênua dentro de um lock. Correta com UMA instância da API; quebra com 2+ instâncias",
                    capacidade, compradores, grau, ComprarComLockLocal, ct),

                await Rodar("3. CONCORRÊNCIA OTIMISTA (xmin + retry)",
                    "O caminho do DOMÍNIO (Partida.ReservarIngressos + SaveChanges). Conflito => relê e tenta de novo",
                    capacidade, compradores, grau, ComprarOtimista, ct),

                await Rodar("4. CONCORRÊNCIA PESSIMISTA (SELECT ... FOR UPDATE)",
                    "Trava a linha no banco durante a transação: os demais ESPERAM a vez. Sem retry, mas serializa",
                    capacidade, compradores, grau, ComprarPessimista, ct),

                await Rodar("5. UPDATE ATÔMICO CONDICIONAL",
                    "UPDATE ... SET d = d - 1 WHERE d >= 1: o banco decide, em 1 comando. Mais simples e mais rápido",
                    capacidade, compradores, grau, ComprarAtomico, ct)
            };

            return new ExperimentoResultado("Competição por ingressos: 5 estratégias de concorrência",
                "Lost update, overselling, lock em processo, concorrência otimista (xmin), pessimista (FOR UPDATE), UPDATE atômico",
                medicoes,
                new[]
                {
                    "Veja 'correto' em cada estratégia. A INGÊNUA vende mais do que existe (sucessos > capacidade): é o bug clássico de race condition.",
                    "O LOCK EM PROCESSO resolve em uma instância, mas não protege contra 2 instâncias da API (cada uma tem o seu lock) - por isso o banco precisa ser a fonte da verdade.",
                    "OTIMISTA: assume que conflitos são raros; detecta no UPDATE (WHERE xmin = ...) e repete. Ótima com pouca disputa; com MUITA disputa gasta tentativas (veja 'conflitos').",
                    "PESSIMISTA: assume que haverá conflito e trava antes. Sem retries, mas mantém conexão/transação aberta e pode causar deadlock se travar várias linhas em ordens diferentes.",
                    "ATÔMICO: empurra a regra para o SQL. Perde o encapsulamento do domínio (a regra 'Disponiveis >= qtd' passa a existir também no SQL), mas é o mais eficiente em hot spots.",
                    "Rede de segurança final: a CHECK constraint CK_PartidaSetores_Disponiveis no banco recusa estoque negativo mesmo se o código falhar.",
                    "No fluxo real do sistema a reserva usa a estratégia OTIMISTA dentro do consumidor RabbitMQ (ver ReservarIngressosIntegrationEventHandler)."
                });
        }

        private async Task<Medicao> Rodar(string nome, string descricao, int capacidade, int compradores, int grau,
            Func<IServiceProvider, Guid, Placar, CancellationToken, Task> comprar, CancellationToken ct)
        {
            var (partidaId, setorId) = await CriarCenarioAsync(capacidade, ct);
            try
            {
                var placar = new Placar();
                var relogio = Stopwatch.StartNew();

                await Parallel.ForEachAsync(Enumerable.Range(0, compradores),
                    new ParallelOptions { MaxDegreeOfParallelism = grau, CancellationToken = ct },
                    async (_, token) =>
                    {
                        // 1 ESCOPO (e portanto 1 DbContext) por comprador: cada um tem a sua "requisição".
                        await using var escopo = _scopes.CreateAsyncScope();
                        try { await comprar(escopo.ServiceProvider, setorId, placar, token); }
                        catch (DbUpdateException) { Interlocked.Increment(ref placar.Erros); }
                    });

                relogio.Stop();

                await using var leitura = _scopes.CreateAsyncScope();
                var db = leitura.ServiceProvider.GetRequiredService<GaloDbContext>();
                var disponiveis = await db.Set<SetorPartida>().Where(s => s.Id == setorId).Select(s => s.Disponiveis).SingleAsync(ct);

                var vendidasPeloEstoque = capacidade - disponiveis;
                return new Medicao(nome, descricao, Math.Round(relogio.Elapsed.TotalMilliseconds, 1), 0, 0, new Dictionary<string, object?>
                {
                    ["capacidade"] = capacidade,
                    ["compradores"] = compradores,
                    ["vendasConfirmadas(sucessos)"] = placar.Sucessos,
                    ["recusadasPorEsgotado"] = placar.Esgotados,
                    ["conflitosDetectados(retries)"] = placar.Conflitos,
                    ["errosDeBanco"] = placar.Erros,
                    ["disponiveisNoBanco"] = disponiveis,
                    ["baixasNoEstoque"] = vendidasPeloEstoque,
                    ["vendasPerdidas(sucessos-baixas)"] = placar.Sucessos - vendidasPeloEstoque,
                    ["correto"] = placar.Sucessos == capacidade && disponiveis == 0 && placar.Sucessos == vendidasPeloEstoque
                });
            }
            finally
            {
                await RemoverCenarioAsync(partidaId);
            }
        }

        // ---------- estratégia 1: ingênua ----------
        private static async Task ComprarIngenuo(IServiceProvider sp, Guid setorId, Placar placar, CancellationToken ct)
        {
            var db = sp.GetRequiredService<GaloDbContext>();

            var atual = (await db.Database
                .SqlQuery<int>($"SELECT \"Disponiveis\" AS \"Value\" FROM \"PartidaSetores\" WHERE \"Id\" = {setorId}")
                .ToListAsync(ct))[0];

            if (atual <= 0) { Interlocked.Increment(ref placar.Esgotados); return; }

            await Task.Delay(Random.Shared.Next(1, 6), ct);   // "processamento" entre ler e gravar: alarga a janela da corrida

            // grava o valor CALCULADO NA MEMÓRIA (baseado numa leitura que pode já estar velha) => lost update
            await db.Database.ExecuteSqlAsync($"UPDATE \"PartidaSetores\" SET \"Disponiveis\" = {atual - 1} WHERE \"Id\" = {setorId}", ct);
            Interlocked.Increment(ref placar.Sucessos);
        }

        // ---------- estratégia 2: lock em processo ----------
        private static readonly SemaphoreSlim TravaLocal = new(1, 1);

        private static async Task ComprarComLockLocal(IServiceProvider sp, Guid setorId, Placar placar, CancellationToken ct)
        {
            await TravaLocal.WaitAsync(ct);    // em código async: SemaphoreSlim (e não lock)
            try { await ComprarIngenuo(sp, setorId, placar, ct); }
            finally { TravaLocal.Release(); }
        }

        // ---------- estratégia 3: otimista (o caminho do domínio) ----------
        private static async Task ComprarOtimista(IServiceProvider sp, Guid setorId, Placar placar, CancellationToken ct)
        {
            // Cada tentativa usa um DbContext NOVO: depois de um conflito o tracker está velho.
            for (var tentativa = 0; tentativa < 30; tentativa++)
            {
                await using var escopo = sp.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
                var db = escopo.ServiceProvider.GetRequiredService<GaloDbContext>();

                var partidaId = await db.Set<SetorPartida>().Where(s => s.Id == setorId).Select(s => s.PartidaId).SingleAsync(ct);
                var partida = await db.Partidas.Include(p => p.Setores).AsSplitQuery().SingleAsync(p => p.Id == partidaId, ct);

                try
                {
                    partida.ReservarIngressos(NomeSetor, 1);          // regra do DOMÍNIO (lança DomainException se esgotado)
                }
                catch (DomainException)
                {
                    Interlocked.Increment(ref placar.Esgotados);
                    return;
                }

                try
                {
                    await Task.Delay(Random.Shared.Next(1, 6), ct);    // mesma janela da estratégia ingênua, para comparar justo
                    await db.SaveChangesAsync(ct);                     // UPDATE ... WHERE Id = @id AND xmin = @lido
                    Interlocked.Increment(ref placar.Sucessos);
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {
                    Interlocked.Increment(ref placar.Conflitos);       // alguém gravou antes: tenta de novo com dado fresco
                    await Task.Delay(Random.Shared.Next(1, 15), ct);   // backoff com jitter evita novo choque simultâneo
                }
            }

            Interlocked.Increment(ref placar.Erros);                   // desistiu após N tentativas
        }

        // ---------- estratégia 4: pessimista ----------
        private static async Task ComprarPessimista(IServiceProvider sp, Guid setorId, Placar placar, CancellationToken ct)
        {
            var db = sp.GetRequiredService<GaloDbContext>();
            var estrategia = db.Database.CreateExecutionStrategy();

            await estrategia.ExecuteAsync(async () =>
            {
                await using var tx = await db.Database.BeginTransactionAsync(ct);

                // FOR UPDATE: a linha fica TRAVADA até o fim da transação. Quem chegar depois espera aqui.
                var atual = (await db.Database
                    .SqlQuery<int>($"SELECT \"Disponiveis\" AS \"Value\" FROM \"PartidaSetores\" WHERE \"Id\" = {setorId} FOR UPDATE")
                    .ToListAsync(ct))[0];

                if (atual <= 0)
                {
                    await tx.RollbackAsync(ct);
                    Interlocked.Increment(ref placar.Esgotados);
                    return;
                }

                await Task.Delay(Random.Shared.Next(1, 6), ct);
                await db.Database.ExecuteSqlAsync($"UPDATE \"PartidaSetores\" SET \"Disponiveis\" = {atual - 1} WHERE \"Id\" = {setorId}", ct);
                await tx.CommitAsync(ct);
                Interlocked.Increment(ref placar.Sucessos);
            });
        }

        // ---------- estratégia 5: UPDATE atômico ----------
        private static async Task ComprarAtomico(IServiceProvider sp, Guid setorId, Placar placar, CancellationToken ct)
        {
            var db = sp.GetRequiredService<GaloDbContext>();

            await Task.Delay(Random.Shared.Next(1, 6), ct);

            // O BANCO avalia "d >= 1" e decrementa na MESMA instrução, sob lock de linha implícito do UPDATE.
            var linhas = await db.Set<SetorPartida>()
                .Where(s => s.Id == setorId && s.Disponiveis >= 1)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Disponiveis, x => x.Disponiveis - 1), ct);

            if (linhas == 1) Interlocked.Increment(ref placar.Sucessos);
            else Interlocked.Increment(ref placar.Esgotados);
        }

        // ================================================================================================
        // 2) O CONFLITO, PASSO A PASSO (didático)
        // ================================================================================================
        public async Task<ExperimentoResultado> ConflitoPassoAPassoAsync(CancellationToken ct)
        {
            var (partidaId, setorId) = await CriarCenarioAsync(1, ct);
            var passos = new List<string>();
            try
            {
                await using var escopoA = _scopes.CreateAsyncScope();
                await using var escopoB = _scopes.CreateAsyncScope();
                var dbA = escopoA.ServiceProvider.GetRequiredService<GaloDbContext>();
                var dbB = escopoB.ServiceProvider.GetRequiredService<GaloDbContext>();

                var partidaA = await dbA.Partidas.Include(p => p.Setores).AsSplitQuery().SingleAsync(p => p.Id == partidaId, ct);
                var partidaB = await dbB.Partidas.Include(p => p.Setores).AsSplitQuery().SingleAsync(p => p.Id == partidaId, ct);
                passos.Add($"1. Torcedor A e Torcedor B leem o setor '{NomeSetor}' (Disponiveis = {partidaA.ObterSetor(NomeSetor).Disponiveis}). Ambos guardam a MESMA versão da linha (xmin).");

                partidaA.ReservarIngressos(NomeSetor, 1);
                partidaB.ReservarIngressos(NomeSetor, 1);
                passos.Add("2. Os dois, em memória, reservam o último ingresso (cada um acha que ainda existe).");

                await dbA.SaveChangesAsync(ct);
                passos.Add("3. A grava primeiro: UPDATE ... WHERE Id = @id AND xmin = @versaoLida -> 1 linha afetada. A versão (xmin) da linha MUDA.");

                try
                {
                    await dbB.SaveChangesAsync(ct);
                    passos.Add("4. B gravou (inesperado: não deveria).");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    passos.Add($"4. B tenta gravar com a versão VELHA -> 0 linhas afetadas -> DbUpdateConcurrencyException ({ex.Entries.Count} entrada(s) em conflito).");
                }

                dbB.ChangeTracker.Clear();
                var releitura = await dbB.Partidas.Include(p => p.Setores).AsSplitQuery().SingleAsync(p => p.Id == partidaId, ct);
                passos.Add($"5. B relê do banco (Disponiveis = {releitura.ObterSetor(NomeSetor).Disponiveis}) e agora a REGRA DO DOMÍNIO recusa:");
                try { releitura.ReservarIngressos(NomeSetor, 1); }
                catch (DomainException ex) { passos.Add($"   -> DomainException: \"{ex.Message}\""); }
            }
            finally
            {
                await RemoverCenarioAsync(partidaId);
            }

            return new ExperimentoResultado("Concorrência otimista, passo a passo", "xmin, DbUpdateConcurrencyException, retry",
                new[] { new Medicao("Sequência", "Dois contextos disputando o último ingresso", 0, 0, 0, new Dictionary<string, object?> { ["passos"] = passos }) },
                new[]
                {
                    "xmin é uma coluna de sistema do PostgreSQL que muda a cada UPDATE da linha: serve como 'número de versão' sem coluna extra.",
                    "O EF inclui 'AND xmin = @lido' no WHERE do UPDATE. Linhas afetadas = 0 => alguém alterou => exceção de concorrência.",
                    "A resposta correta ao conflito é RELER e reaplicar a regra (não simplesmente repetir o UPDATE): o dado mudou e a decisão pode ser outra."
                });
        }

        // ================================================================================================
        // 3) FLUXO COMPLETO (API -> Postgres -> RabbitMQ -> consumidor -> Postgres)
        // ================================================================================================
        public async Task<ExperimentoResultado> FluxoCompletoAsync(int capacidade, int compradores, CancellationToken ct)
        {
            capacidade = Math.Clamp(capacidade, 1, 100);
            compradores = Math.Clamp(compradores, 2, 200);

            var (partidaId, setorId) = await CriarCenarioAsync(capacidade, ct);
            var detalhes = new Dictionary<string, object?>();
            var relogio = Stopwatch.StartNew();
            try
            {
                List<Guid> torcedores;
                await using (var escopo = _scopes.CreateAsyncScope())
                {
                    var db = escopo.ServiceProvider.GetRequiredService<GaloQueryDbContext>();
                    torcedores = await db.Torcedores.Where(t => t.Status == "Ativo").OrderBy(t => t.Id).Select(t => t.Id).Take(compradores).ToListAsync(ct);
                }
                if (torcedores.Count < 2) throw new InvalidOperationException("Poucos torcedores. Execute POST /api/v1/estudos/seed antes.");

                // 1) cada torcedor cria a compra e inicia o pagamento (em paralelo) -> publica no RabbitMQ
                var recusadasNaCriacao = 0;
                await Parallel.ForEachAsync(torcedores, new ParallelOptions { MaxDegreeOfParallelism = 16, CancellationToken = ct }, async (torcedorId, token) =>
                {
                    await using var escopo = _scopes.CreateAsyncScope();
                    var sender = escopo.ServiceProvider.GetRequiredService<ISender>();
                    try
                    {
                        var compraId = await sender.Send(new CriarCompraCommand(torcedorId, partidaId, new[] { new ItemCompraRequest(NomeSetor, 1) }), token);
                        await sender.Send(new IniciarPagamentoCommand(compraId, MetodoPagamento.Pix), token);
                    }
                    catch (DomainException)
                    {
                        // A checagem RÁPIDA de estoque (antes de criar a compra) já viu o setor esgotado: falha cedo, sem passar pela fila.
                        // Só acontece para quem chega DEPOIS que as reservas dos primeiros já foram processadas pelo consumidor.
                        Interlocked.Increment(ref recusadasNaCriacao);
                    }
                });
                var comprasCriadas = torcedores.Count - recusadasNaCriacao;
                detalhes["tentativasDeCompra"] = torcedores.Count;
                detalhes["recusadasNaCriacao(estoque ja esgotado)"] = recusadasNaCriacao;
                detalhes["comprasEnviadasParaFila"] = comprasCriadas;
                detalhes["publicadasEmMs"] = Math.Round(relogio.Elapsed.TotalMilliseconds, 0);

                // 2) espera o CONSUMIDOR (RabbitMQ) processar todas: confirmada (reserva ok) ou cancelada (esgotado)
                var limite = DateTime.UtcNow.AddSeconds(40);
                int confirmadas = 0, canceladas = 0, pendentes = comprasCriadas;
                while (DateTime.UtcNow < limite)
                {
                    await using var escopo = _scopes.CreateAsyncScope();
                    var db = escopo.ServiceProvider.GetRequiredService<GaloQueryDbContext>();
                    var compras = await db.Compras.Where(c => c.PartidaId == partidaId).Select(c => new { c.Status, c.ReservaConfirmada }).ToListAsync(ct);
                    confirmadas = compras.Count(c => c.ReservaConfirmada);
                    canceladas = compras.Count(c => c.Status == "Cancelada");
                    pendentes = compras.Count - confirmadas - canceladas;
                    if (pendentes == 0 && compras.Count == comprasCriadas) break;
                    await Task.Delay(200, ct);
                }
                detalhes["processadasEmMs(total)"] = Math.Round(relogio.Elapsed.TotalMilliseconds, 0);

                await using var leitura = _scopes.CreateAsyncScope();
                var disponiveis = await leitura.ServiceProvider.GetRequiredService<GaloQueryDbContext>()
                    .Setores.Where(s => s.Id == setorId).Select(s => s.Disponiveis).SingleAsync(ct);

                detalhes["capacidade"] = capacidade;
                detalhes["reservasConfirmadas"] = confirmadas;
                detalhes["canceladasPorEsgotado"] = canceladas;
                detalhes["aindaPendentes"] = pendentes;
                detalhes["disponiveisNoBanco"] = disponiveis;
                detalhes["correto"] = pendentes == 0 && confirmadas == Math.Min(capacidade, torcedores.Count) && disponiveis == Math.Max(0, capacidade - confirmadas);
            }
            finally
            {
                await RemoverCenarioAsync(partidaId);
            }

            return new ExperimentoResultado("Fluxo completo assíncrono: API -> RabbitMQ -> consumidor -> PostgreSQL",
                "Consistência eventual, mensageria, concorrência otimista no consumidor, compensação (cancelamento)",
                new[] { new Medicao("Disputa pelos últimos ingressos via mensageria", $"{compradores} compras para {capacidade} vagas", Math.Round(relogio.Elapsed.TotalMilliseconds, 0), 0, 0, detalhes) },
                new[]
                {
                    "Cada compra publica 'ReservarIngressos' no RabbitMQ; o consumidor (prefetch > 1 => várias mensagens em paralelo) disputa o mesmo setor com concorrência otimista.",
                    "Quem perde a disputa por ESGOTAMENTO tem a compra cancelada automaticamente (compensação), em vez de ficar presa: consistência EVENTUAL.",
                    "'correto' = confirmadas == capacidade, disponíveis == 0 e nenhuma compra pendente.",
                    "Veja o painel do RabbitMQ (se tiver acesso) e os spans no tracing: publish -> consume -> SQL, ligados pelo traceparent."
                });
        }

        // ================================================================================================
        // helpers: cenário descartável
        // ================================================================================================
        private async Task<(Guid PartidaId, Guid SetorId)> CriarCenarioAsync(int capacidade, CancellationToken ct)
        {
            await using var escopo = _scopes.CreateAsyncScope();
            var db = escopo.ServiceProvider.GetRequiredService<GaloDbContext>();

            var campeonato = await db.Campeonatos.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync(ct);
            var clubes = await db.Clubes.AsNoTracking().OrderBy(c => c.Id).Take(2).ToListAsync(ct);
            var estadio = await db.Estadios.AsNoTracking().OrderBy(e => e.Id).FirstOrDefaultAsync(ct);
            if (campeonato is null || clubes.Count < 2 || estadio is null)
                throw new InvalidOperationException("Sem dados base. Execute POST /api/v1/estudos/seed antes.");

            var partida = Partida.Agendar(campeonato.Id, clubes[0].Id, clubes[1].Id, estadio.Id, DateTime.UtcNow.AddDays(30), 99);
            var setorId = partida.AdicionarSetor(NomeSetor, capacidade, 10m);
            partida.ClearDomainEvents();

            db.Partidas.Add(partida);
            await db.SaveChangesAsync(ct);
            return (partida.Id, setorId);
        }

        private async Task RemoverCenarioAsync(Guid partidaId)
        {
            await using var escopo = _scopes.CreateAsyncScope();
            var db = escopo.ServiceProvider.GetRequiredService<GaloDbContext>();

            // compras primeiro (FK Restrict); itens/pagamentos caem em cascata. Depois a partida (setores/gols em cascata).
            await db.Compras.Where(c => c.PartidaId == partidaId).ExecuteDeleteAsync();
            await db.Partidas.Where(p => p.Id == partidaId).ExecuteDeleteAsync();
        }
    }
}
