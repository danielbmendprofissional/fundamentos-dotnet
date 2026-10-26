using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Galo.Application.Common.Telemetry
{
    /// <summary>
    /// Ponto único de instrumentação própria (OpenTelemetry):
    ///  - ActivitySource => TRACES (spans que aparecem no Jaeger/Tempo)
    ///  - Meter          => MÉTRICAS (contadores/histogramas expostos em /metrics e via OTLP)
    /// A Presentation registra o nome "Galo" no AddSource/AddMeter.
    /// </summary>
    public static class GaloTelemetry
    {
        public const string Name = "Galo";

        public static readonly ActivitySource Source = new(Name);
        public static readonly Meter Meter = new(Name);

        public static readonly Counter<long> Requisicoes =
            Meter.CreateCounter<long>("galo.mediator.requests", description: "Comandos/queries executados");

        public static readonly Histogram<double> DuracaoMs =
            Meter.CreateHistogram<double>("galo.mediator.duration", unit: "ms", description: "Duração dos comandos/queries");

        public static readonly Counter<long> ConflitosConcorrencia =
            Meter.CreateCounter<long>("galo.concurrency.conflicts", description: "Conflitos de concorrência otimista");

        public static readonly Counter<long> CacheHits =
            Meter.CreateCounter<long>("galo.cache.hits", description: "Hits de cache (tag nivel=L1|L2)");

        public static readonly Counter<long> CacheMisses =
            Meter.CreateCounter<long>("galo.cache.misses", description: "Misses de cache");
    }
}
