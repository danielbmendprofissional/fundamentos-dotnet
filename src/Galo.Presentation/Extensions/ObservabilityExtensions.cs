using Galo.Application.Common.Telemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Galo.Presentation.Extensions
{
    /// <summary>
    /// OBSERVABILIDADE = os 3 sinais: LOGS + TRACES + MÉTRICAS, todos via OpenTelemetry.
    ///
    ///  - Exportação OTLP (http/protobuf) para o coletor: o endpoint/protocolo vêm de OTEL_EXPORTER_OTLP_ENDPOINT /
    ///    OTEL_EXPORTER_OTLP_PROTOCOL, que estão nos user-secrets (IConfiguration). O SDK do OpenTelemetry lê
    ///    essas chaves do IConfiguration do host, então não precisamos repeti-las no código.
    ///  - /metrics (formato Prometheus) para o scrape do Prometheus na porta 5205.
    ///  - Resource: service.name + service.namespace=estudos (+ o que vier em OTEL_RESOURCE_ATTRIBUTES).
    /// </summary>
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddGaloObservability(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: "galo-api", serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString());

                    // O detector de ambiente do SDK só lê variáveis de AMBIENTE; como a chave está no IConfiguration
                    // (user-secrets), lemos e aplicamos aqui. Formato: "chave=valor,chave2=valor2".
                    var atributos = ParseAtributos(configuration["OTEL_RESOURCE_ATTRIBUTES"]);
                    if (!atributos.ContainsKey("service.namespace")) atributos["service.namespace"] = "estudos";
                    resource.AddAttributes(atributos.Select(a => new KeyValuePair<string, object>(a.Key, a.Value)));
                })
                .WithTracing(tracing => tracing
                    .AddSource(GaloTelemetry.Name)                       // nossos spans (MediatR, RabbitMQ, laboratório)
                    .AddAspNetCoreInstrumentation(o =>
                        o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health")
                                       && !ctx.Request.Path.StartsWithSegments("/metrics"))
                    .AddHttpClientInstrumentation()
                    .AddSource("Npgsql")                                  // 1 span por comando SQL (o Npgsql já emite via ActivitySource)
                    .AddOtlpExporter())
                .WithMetrics(metrics => metrics
                    .AddMeter(GaloTelemetry.Name)                         // nossos contadores/histogramas
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()                          // GC, threadpool, alocações...
                    .AddMeter("Npgsql")                                   // métricas do driver (conexões, comandos)
                    .AddPrometheusExporter()                              // alimenta o /metrics
                    .AddOtlpExporter())
                .WithLogging(logging => logging.AddOtlpExporter(),
                    options =>
                    {
                        options.IncludeFormattedMessage = true;
                        options.IncludeScopes = true;
                    });

            return services;
        }

        private static Dictionary<string, string> ParseAtributos(string? bruto)
        {
            var resultado = new Dictionary<string, string>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(bruto)) return resultado;

            foreach (var par in bruto.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var i = par.IndexOf('=');
                if (i > 0) resultado[par[..i].Trim()] = Uri.UnescapeDataString(par[(i + 1)..].Trim());
            }
            return resultado;
        }
    }
}
