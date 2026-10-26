using System.Diagnostics;
using Galo.Infrastructure.Persistence.Diagnostics;

namespace Galo.Estudos.Common
{
    /// <summary>
    /// Mede tempo, memória alocada e comandos SQL de um trecho de código.
    ///
    /// COMO LER OS NÚMEROS (importante para não tirar conclusão errada):
    ///  - DuracaoMs: Stopwatch (relógio de alta resolução).
    ///  - AlocadoKb:
    ///      * versão SÍNCRONA  -> GC.GetAllocatedBytesForCurrentThread(): preciso, só da thread que executou.
    ///      * versão ASSÍNCRONA -> GC.GetTotalAllocatedBytes(): soma de TODAS as threads do processo (a continuação de
    ///        um await pode rodar em outra thread). Como a API tem outras threads vivas (consumidor RabbitMQ, exporters),
    ///        o valor é APROXIMADO. Use para comparar ordens de grandeza, não para medir bytes exatos.
    ///  - Aquecimento (warm-up): a 1ª execução inclui JIT, abertura de conexão e plano de query. Por isso cada experimento
    ///    executa o trecho uma vez SEM medir e depois mede. Em benchmark sério use BenchmarkDotNet.
    /// </summary>
    public static class Medidor
    {
        public static async Task<Medicao> MedirAsync(string nome, string descricao,
            Func<CancellationToken, Task<IDictionary<string, object?>?>> acao, CancellationToken ct, bool aquecer = true)
        {
            if (aquecer) await acao(ct);

            GC.Collect();                          // começa de um estado "limpo" para a medição ser comparável
            GC.WaitForPendingFinalizers();

            using var sql = ContadorSql.Iniciar();
            var alocInicio = GC.GetTotalAllocatedBytes(precise: false);
            var relogio = Stopwatch.StartNew();

            var detalhes = await acao(ct) ?? new Dictionary<string, object?>();

            relogio.Stop();
            var alocado = GC.GetTotalAllocatedBytes(precise: false) - alocInicio;

            return new Medicao(nome, descricao, Math.Round(relogio.Elapsed.TotalMilliseconds, 2), Math.Round(alocado / 1024.0, 1),
                sql.Comandos, new Dictionary<string, object?>(detalhes), sql.Comandos > 0 ? sql.Amostras : null);
        }

        public static Medicao Medir(string nome, string descricao, Func<IDictionary<string, object?>?> acao, bool aquecer = true)
        {
            if (aquecer) acao();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);
            var alocInicio = GC.GetAllocatedBytesForCurrentThread();
            var relogio = Stopwatch.StartNew();

            var detalhes = acao() ?? new Dictionary<string, object?>();

            relogio.Stop();
            var alocado = GC.GetAllocatedBytesForCurrentThread() - alocInicio;

            var extras = new Dictionary<string, object?>(detalhes)
            {
                ["coletasGC(gen0/gen1/gen2)"] = $"{GC.CollectionCount(0) - gen0}/{GC.CollectionCount(1) - gen1}/{GC.CollectionCount(2) - gen2}"
            };

            return new Medicao(nome, descricao, Math.Round(relogio.Elapsed.TotalMilliseconds, 3), Math.Round(alocado / 1024.0, 1), 0, extras);
        }
    }
}
