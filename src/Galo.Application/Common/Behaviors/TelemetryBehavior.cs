using System.Diagnostics;
using Galo.Application.Common.Telemetry;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Galo.Application.Common.Behaviors
{
    /// <summary>
    /// PIPELINE BEHAVIOR do MediatR: um "middleware" que envolve TODO comando/query.
    /// Aqui: cria um span, mede a duração e conta as execuções - sem tocar em nenhum handler.
    /// </summary>
    public sealed class TelemetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TelemetryBehavior<TRequest, TResponse>> _logger;

        public TelemetryBehavior(ILogger<TelemetryBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var nome = typeof(TRequest).Name;
            var tipo = nome.EndsWith("Query", StringComparison.Ordinal) ? "query" : "command";

            using var activity = GaloTelemetry.Source.StartActivity($"mediator {nome}");
            activity?.SetTag("galo.request.type", tipo);

            var inicio = Stopwatch.GetTimestamp();
            var resultado = "ok";
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception ex)
            {
                resultado = "erro";
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogWarning("{Tipo} {Request} falhou: {Mensagem}", tipo, nome, ex.Message);
                throw;
            }
            finally
            {
                var ms = Stopwatch.GetElapsedTime(inicio).TotalMilliseconds;
                var tags = new TagList { { "request", nome }, { "tipo", tipo }, { "resultado", resultado } };
                GaloTelemetry.Requisicoes.Add(1, tags);
                GaloTelemetry.DuracaoMs.Record(ms, tags);
            }
        }
    }
}
