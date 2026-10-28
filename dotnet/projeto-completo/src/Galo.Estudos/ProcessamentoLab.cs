using System.Diagnostics;
using System.Threading.Channels;
using Galo.Estudos.Common;
using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Galo.Estudos
{
    /// <summary>
    /// LABORATÓRIO DE PROCESSAMENTO (pipelines, backpressure, streaming).
    /// Veja também o processamento em SEGUNDO PLANO (BackgroundService + fila) em Jobs/ e /estudos/processamento/jobs.
    /// </summary>
    public sealed class ProcessamentoLab
    {
        private readonly IDbContextFactory<GaloQueryDbContext> _factory;
        public ProcessamentoLab(IDbContextFactory<GaloQueryDbContext> factory) => _factory = factory;

        // ================================================================================================
        // 1) PRODUTOR/CONSUMIDOR com Channels: fila ilimitada x limitada (backpressure)
        // ================================================================================================
        public async Task<ExperimentoResultado> ChannelsAsync(int itens, int consumidores, CancellationToken ct)
        {
            itens = Math.Clamp(itens, 100, 5_000);
            consumidores = Math.Clamp(consumidores, 1, 16);

            // produtor RÁPIDO (sem espera) e consumidores LENTOS (2 ms por item): o desequilíbrio clássico.
            async Task<IDictionary<string, object?>?> Executar(Channel<int> canal, CancellationToken t)
            {
                var picoFila = 0;
                var processados = 0;
                var bloqueiosDoProdutor = 0;
                var relogio = Stopwatch.StartNew();

                var tarefasConsumidor = Enumerable.Range(0, consumidores).Select(_ => Task.Run(async () =>
                {
                    await foreach (var _ in canal.Reader.ReadAllAsync(t))
                    {
                        await Task.Delay(2, t);                       // "trabalho" do consumidor
                        Interlocked.Increment(ref processados);
                    }
                }, t)).ToArray();

                for (var i = 0; i < itens; i++)
                {
                    // TryWrite devolve false quando a fila LIMITADA está cheia: o produtor vai precisar esperar.
                    if (!canal.Writer.TryWrite(i))
                    {
                        Interlocked.Increment(ref bloqueiosDoProdutor);
                        await canal.Writer.WriteAsync(i, t);
                    }
                    picoFila = Math.Max(picoFila, canal.Reader.Count);
                }
                canal.Writer.Complete();
                var tempoProdutor = relogio.Elapsed.TotalMilliseconds;

                await Task.WhenAll(tarefasConsumidor);

                return new Dictionary<string, object?>
                {
                    ["itensProcessados"] = processados,
                    ["picoDaFila"] = picoFila,
                    ["vezesQueOProdutorEsperou"] = bloqueiosDoProdutor,
                    ["produtorTerminouEmMs"] = Math.Round(tempoProdutor, 1)
                };
            }

            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. Channel ILIMITADO", "O produtor nunca espera: a fila cresce até estourar a memória se o consumidor for mais lento",
                    t => Executar(Channel.CreateUnbounded<int>(new UnboundedChannelOptions { SingleWriter = true }), t), ct, aquecer: false),

                await Medidor.MedirAsync("2. Channel LIMITADO (capacidade 50, Wait)", "BACKPRESSURE: com a fila cheia o produtor é suspenso até abrir espaço",
                    t => Executar(Channel.CreateBounded<int>(new BoundedChannelOptions(50) { FullMode = BoundedChannelFullMode.Wait, SingleWriter = true }), t), ct, aquecer: false)
            };

            return new ExperimentoResultado("Produtor/Consumidor com Channels",
                "Backpressure, fila limitada x ilimitada, throughput x memória",
                medicoes,
                new[]
                {
                    "Compare 'picoDaFila': ilimitado acumula quase tudo; limitado nunca passa da capacidade.",
                    "O TEMPO TOTAL é praticamente o mesmo (quem manda é a velocidade do consumidor). O que muda é o uso de memória e a previsibilidade.",
                    "Backpressure é o que impede um produtor rápido de derrubar um sistema lento. Em APIs: fila limitada + 429/503 quando saturar.",
                    "Mais consumidores aumentam o throughput até o limite do recurso (CPU, banco). Teste com consumidores=1, 4, 8."
                });
        }

        // ================================================================================================
        // 2) STREAMING x CARREGAR TUDO
        // ================================================================================================
        public async Task<ExperimentoResultado> StreamingAsync(CancellationToken ct)
        {
            var medicoes = new List<Medicao>
            {
                await Medidor.MedirAsync("1. ToListAsync() e depois agrega", "Materializa TODAS as linhas na memória antes de processar",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var todos = await db.Torcedores.ToListAsync(t);
                        var diamantes = todos.Count(x => x.Plano == "Diamante");
                        return new Dictionary<string, object?> { ["linhasNaMemoria"] = todos.Count, ["diamantes"] = diamantes };
                    }, ct),

                await Medidor.MedirAsync("2. AsAsyncEnumerable() (streaming)", "Processa linha a linha enquanto o banco envia: memória constante",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var total = 0; var diamantes = 0;
                        await foreach (var x in db.Torcedores.AsAsyncEnumerable().WithCancellation(t))
                        {
                            total++;
                            if (x.Plano == "Diamante") diamantes++;
                        }
                        return new Dictionary<string, object?> { ["linhasProcessadas"] = total, ["diamantes"] = diamantes, ["linhasNaMemoriaAoMesmoTempo"] = 1 };
                    }, ct),

                await Medidor.MedirAsync("3. Agregação no banco (GroupBy)", "O melhor processamento é o que NÃO sai do banco",
                    async t =>
                    {
                        await using var db = await _factory.CreateDbContextAsync(t);
                        var grupos = await db.Torcedores.GroupBy(x => x.Plano).Select(g => new { g.Key, Qtd = g.Count() }).ToListAsync(t);
                        return new Dictionary<string, object?> { ["grupos"] = grupos.ToDictionary(g => g.Key, g => g.Qtd) };
                    }, ct)
            };

            return new ExperimentoResultado("Streaming x carregar tudo x agregar no banco",
                "IAsyncEnumerable, memória constante, processamento em lote",
                medicoes,
                new[]
                {
                    "Streaming mantém UMA linha na memória por vez: processa milhões de linhas com heap pequeno (mas a conexão fica ocupada durante todo o processamento).",
                    "Se o cálculo cabe em SQL (GroupBy/SUM/COUNT), é quase sempre a melhor opção: nada trafega.",
                    "Para exportações/relatórios grandes, combine streaming + escrita incremental no response (sem montar o arquivo na memória)."
                });
        }
    }
}
