using System.Diagnostics;
using Galo.Estudos.Common;
using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE PARALELISMO E CONCORRÊNCIA DE THREADS.
    ///
    /// Vocabulário (cai muito em entrevista):
    ///  - ASSÍNCRONO (async/await): não BLOQUEIA a thread enquanto espera I/O (banco, rede, disco). Uma thread atende
    ///    milhares de requisições. Melhora ESCALABILIDADE, não deixa o CPU mais rápido.
    ///  - CONCORRÊNCIA: várias tarefas em andamento ao mesmo tempo (podem se alternar numa só thread).
    ///  - PARALELISMO: várias tarefas executando literalmente no MESMO instante, em núcleos diferentes. Só ajuda trabalho
    ///    CPU-bound (cálculo). Para I/O-bound, o ganho vem de ter várias requisições EM VOO (Task.WhenAll).
    /// </summary>
    public sealed class ParalelismoLab
    {
        private readonly IDbContextFactory<GaloQueryDbContext> _factory;

        public ParalelismoLab(IDbContextFactory<GaloQueryDbContext> factory) => _factory = factory;

        // ================================================================================================
        // 1) I/O-BOUND: várias consultas independentes ao banco
        // ================================================================================================
        public async Task<ExperimentoResultado> IoBoundAsync(int consultas, int grauParalelismo, CancellationToken ct)
        {
            consultas = Math.Clamp(consultas, 4, 100);       // < pool de conexões do Npgsql (padrão: 100)
            grauParalelismo = Math.Clamp(grauParalelismo, 1, 32);

            await using var preparo = await _factory.CreateDbContextAsync(ct);
            var clubes = await preparo.Clubes.Select(c => c.Id).ToListAsync(ct);
            if (clubes.Count == 0) throw new InvalidOperationException("Sem dados. Execute POST /api/v1/estudos/seed antes.");
            var alvos = Enumerable.Range(0, consultas).Select(i => clubes[i % clubes.Count]).ToList();

            // 1 consulta = quantas partidas esse clube mandou
            async Task<int> ContarAsync(Guid clubeId, CancellationToken t)
            {
                await using var db = await _factory.CreateDbContextAsync(t);   // 1 contexto POR tarefa (DbContext NÃO é thread-safe)
                return await db.Partidas.CountAsync(p => p.MandanteId == clubeId, t);
            }

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Sequencial (await em loop)", "Uma consulta de cada vez: o tempo total é a SOMA das latências",
                    async t =>
                    {
                        var total = 0;
                        foreach (var id in alvos) total += await ContarAsync(id, t);
                        return new Dictionary<string, object?> { ["consultas"] = alvos.Count, ["soma"] = total };
                    }, ct),

                await Medidor.MedirAsync("2. Task.WhenAll sem limite", "Dispara todas de uma vez: tempo ≈ a mais lenta. Cuidado: N conexões simultâneas (pool!)",
                    async t =>
                    {
                        var resultados = await Task.WhenAll(alvos.Select(id => ContarAsync(id, t)));
                        return new Dictionary<string, object?> { ["consultas"] = alvos.Count, ["soma"] = resultados.Sum() };
                    }, ct),

                await Medidor.MedirAsync($"3. Parallel.ForEachAsync (grau {grauParalelismo})", "Paralelismo LIMITADO: no máximo N em voo (respeita o pool de conexões)",
                    async t =>
                    {
                        var total = 0;
                        await Parallel.ForEachAsync(alvos, new ParallelOptions { MaxDegreeOfParallelism = grauParalelismo, CancellationToken = t },
                            async (id, token) => { var parcial = await ContarAsync(id, token); Interlocked.Add(ref total, parcial); });
                        return new Dictionary<string, object?> { ["consultas"] = alvos.Count, ["soma"] = total };
                    }, ct)
            };

            // 4) o ERRO clássico: compartilhar um DbContext entre tarefas paralelas
            string erro;
            try
            {
                await using var compartilhado = await _factory.CreateDbContextAsync(ct);
                await Task.WhenAll(alvos.Take(6).Select(id => compartilhado.Partidas.CountAsync(p => p.MandanteId == id, ct)));
                erro = "(não falhou desta vez - é uma condição de corrida; em produção falha de forma intermitente)";
            }
            catch (InvalidOperationException ex)
            {
                erro = ex.Message;
            }
            medicoes.Add(new Medicao("4. ERRO: 1 DbContext em várias tarefas", "Nunca faça isto",
                0, 0, 0, new Dictionary<string, object?> { ["resultado"] = erro }));

            return new ExperimentoResultado("I/O-bound: sequencial x WhenAll x Parallel.ForEachAsync",
                "async/await, concorrência de I/O, pool de conexões, thread-safety do DbContext",
                medicoes,
                new[]
                {
                    "Cada consulta passa a maior parte do tempo ESPERANDO o banco. Colocar várias em voo ao mesmo tempo reduz o tempo total.",
                    "Task.WhenAll sem limite com milhares de itens esgota o pool de conexões (Npgsql: 100 por padrão). Limite com Parallel.ForEachAsync ou SemaphoreSlim.",
                    "DbContext NÃO é thread-safe. A regra é um contexto por tarefa paralela: por isso usamos IDbContextFactory (pooled).",
                    "O ganho tem teto: o banco também tem limite de CPU/conexões. Mais paralelismo ≠ sempre mais rápido."
                });
        }

        // ================================================================================================
        // 2) CPU-BOUND: cálculo puro
        // ================================================================================================
        public Task<ExperimentoResultado> CpuBoundAsync(int limite, CancellationToken ct)
        {
            limite = Math.Clamp(limite, 100_000, 5_000_000);

            // Contar primos até "limite" por divisão: trabalho CPU-bound, sem I/O.
            static bool EhPrimo(int n)
            {
                if (n < 2) return false;
                if (n % 2 == 0) return n == 2;
                for (var i = 3; (long)i * i <= n; i += 2)
                    if (n % i == 0) return false;
                return true;
            }

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. for sequencial", "Uma thread só", () =>
                {
                    var c = 0;
                    for (var n = 0; n <= limite; n++) if (EhPrimo(n)) c++;
                    return new Dictionary<string, object?> { ["primos"] = c };
                }, aquecer: false),

                Medidor.Medir("2. Parallel.For com agregação local", "Cada thread soma num contador LOCAL e junta no final (sem disputa)", () =>
                {
                    var total = 0;
                    Parallel.For(0, limite + 1,
                        () => 0,                                           // localInit: contador por thread
                        (n, _, local) => EhPrimo(n) ? local + 1 : local,   // body
                        local => Interlocked.Add(ref total, local));       // localFinally: junta
                    return new Dictionary<string, object?> { ["primos"] = total };
                }, aquecer: false),

                Medidor.Medir("3. PLINQ (AsParallel)", "LINQ paralelo: particiona a sequência entre os núcleos", () =>
                {
                    var c = Enumerable.Range(0, limite + 1).AsParallel().Count(EhPrimo);
                    return new Dictionary<string, object?> { ["primos"] = c };
                }, aquecer: false),

                Medidor.Medir("4. Parallel.For com Interlocked a cada acerto", "Todas as threads disputam a MESMA variável a cada acerto. Aqui só ~7% dos números são primos, então a disputa é baixa e esta variação até empata; se quase todo item \"acertasse\", o contador compartilhado viraria o gargalo", () =>
                {
                    var total = 0;
                    Parallel.For(0, limite + 1, n => { if (EhPrimo(n)) Interlocked.Increment(ref total); });
                    return new Dictionary<string, object?> { ["primos"] = total };
                }, aquecer: false)
            };

            var resultado = new ExperimentoResultado("CPU-bound: for x Parallel.For x PLINQ",
                "Paralelismo de dados, agregação local, custo de sincronização",
                medicoes,
                new[]
                {
                    $"Núcleos lógicos disponíveis: {Environment.ProcessorCount}. O ganho máximo teórico é ~{Environment.ProcessorCount}x.",
                    "Paralelismo só compensa quando o trabalho por item é GRANDE o bastante. Para itens baratos, o custo de particionar/sincronizar supera o ganho.",
                    "Evite compartilhar estado mutável: prefira agregação local por thread e junte no final. A diferença entre 2 e 4 depende da frequência de acertos: com poucos acertos (como aqui) quase não há disputa; com muitos, o Interlocked compartilhado cai muito.",
                    "Em ASP.NET, cuidado com Parallel.For dentro de requisições: ele disputa as MESMAS threads que atendem outros usuários."
                });

            return Task.FromResult(resultado);
        }

        // ================================================================================================
        // 3) CONDIÇÃO DE CORRIDA (race condition)
        // ================================================================================================
        private sealed class ContadorProtegido
        {
            private readonly object _trava = new();
            public long Valor;
            public void Incrementar() { lock (_trava) Valor++; }
        }

        public Task<ExperimentoResultado> CondicaoDeCorridaAsync(int threads, int incrementosPorThread, CancellationToken ct)
        {
            threads = Math.Clamp(threads, 2, 32);
            incrementosPorThread = Math.Clamp(incrementosPorThread, 10_000, 5_000_000);
            var esperado = (long)threads * incrementosPorThread;

            IDictionary<string, object?> Rodar(Action acao, Func<long> ler)
            {
                var tarefas = Enumerable.Range(0, threads)
                    .Select(_ => Task.Factory.StartNew(() => { for (var i = 0; i < incrementosPorThread; i++) acao(); }, TaskCreationOptions.LongRunning))
                    .ToArray();
                Task.WaitAll(tarefas, ct);
                var obtido = ler();
                return new Dictionary<string, object?>
                {
                    ["esperado"] = esperado,
                    ["obtido"] = obtido,
                    ["incrementosPerdidos"] = esperado - obtido,
                    ["correto"] = obtido == esperado
                };
            }

            long inseguro = 0;
            long atomico = 0;
            var protegido = new ContadorProtegido();

            var medicoes = new List<Medicao>
            {
                Medidor.Medir("1. contador++ SEM proteção", "ler-somar-gravar NÃO é atômico: duas threads leem o mesmo valor e uma atualização se perde",
                    () => Rodar(() => inseguro++, () => inseguro), aquecer: false),

                Medidor.Medir("2. Interlocked.Increment", "Instrução atômica de hardware: correto e sem bloqueio",
                    () => Rodar(() => Interlocked.Increment(ref atomico), () => Interlocked.Read(ref atomico)), aquecer: false),

                Medidor.Medir("3. lock", "Exclusão mútua: correto, mas as threads esperam umas pelas outras",
                    () => Rodar(protegido.Incrementar, () => protegido.Valor), aquecer: false)
            };

            return Task.FromResult(new ExperimentoResultado("Condição de corrida (race condition)",
                "Atomicidade, Interlocked, lock, perda de atualização (lost update)",
                medicoes,
                new[]
                {
                    "A variação 1 quase sempre perde incrementos: é o MESMO bug do 'lost update' do banco (ver /estudos/concorrencia), só que em memória.",
                    "Interlocked é a opção mais barata para contadores/flags. lock serve para proteger blocos maiores de código.",
                    "Em async NÃO use lock em volta de await (não compila por um bom motivo): use SemaphoreSlim.WaitAsync().",
                    "Em vários servidores, lock/SemaphoreSlim só protegem UM processo. Para escala horizontal é preciso proteção no banco/Redis (veja concorrência)."
                }));
        }

        // ================================================================================================
        // 4) THREAD POOL
        // ================================================================================================
        public ExperimentoResultado ThreadPoolInfo()
        {
            ThreadPool.GetMinThreads(out var minW, out var minIo);
            ThreadPool.GetMaxThreads(out var maxW, out var maxIo);
            ThreadPool.GetAvailableThreads(out var dispW, out var dispIo);

            var info = new Dictionary<string, object?>
            {
                ["threadsNoPool"] = ThreadPool.ThreadCount,
                ["itensPendentesNaFila"] = ThreadPool.PendingWorkItemCount,
                ["itensJaConcluidos"] = ThreadPool.CompletedWorkItemCount,
                ["minWorker"] = minW, ["minIO"] = minIo,
                ["maxWorker"] = maxW, ["maxIO"] = maxIo,
                ["workersEmUso"] = maxW - dispW,
                ["ioEmUso"] = maxIo - dispIo,
                ["threadAtualEhDoPool"] = Thread.CurrentThread.IsThreadPoolThread,
                ["processadores"] = Environment.ProcessorCount
            };

            return new ExperimentoResultado("Estado do ThreadPool", "ThreadPool, starvation, min/max threads",
                new[] { new Medicao("ThreadPool agora", "Instantâneo do pool compartilhado por TODAS as requisições", 0, 0, 0, info) },
                new[]
                {
                    "Cada requisição ASP.NET roda em uma thread do pool. Bloquear (.Result, .Wait(), Thread.Sleep) prende essa thread.",
                    "THREAD STARVATION: com muitas requisições bloqueadas, o pool cresce devagar (injeção de ~1-2 threads/s) e a API 'trava' sem usar CPU.",
                    "Por isso: async de ponta a ponta, nunca .Result/.Wait() em código de requisição."
                });
        }
    }
}
