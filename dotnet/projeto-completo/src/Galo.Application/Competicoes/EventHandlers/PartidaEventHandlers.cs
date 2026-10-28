using Galo.Application.Common.Interfaces.Caching;
using Galo.Application.Competicoes.Queries;
using Galo.Domain.Competicoes.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Galo.Application.Competicoes.EventHandlers
{
    // HANDLERS DE EVENTO DE DOMÍNIO: reações a "algo aconteceu", executadas DEPOIS do SaveChanges.
    // Um evento pode ter vários handlers (aqui: cache e log) sem que o domínio saiba de nenhum deles.

    /// <summary>Partida encerrada => a classificação mudou => invalida o cache do grupo.</summary>
    public sealed class PartidaEncerradaInvalidaClassificacaoHandler : INotificationHandler<PartidaEncerradaEvent>
    {
        private readonly ICacheStore _cache;
        private readonly ILogger<PartidaEncerradaInvalidaClassificacaoHandler> _logger;

        public PartidaEncerradaInvalidaClassificacaoHandler(ICacheStore cache, ILogger<PartidaEncerradaInvalidaClassificacaoHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task Handle(PartidaEncerradaEvent notification, CancellationToken cancellationToken)
        {
            await _cache.InvalidateGroupAsync(GruposDeCache.Classificacao, cancellationToken);
            _logger.LogInformation("Partida {PartidaId} encerrada ({Mandante} x {Visitante}): cache de classificação invalidado",
                notification.PartidaId, notification.GolsMandante, notification.GolsVisitante);
        }
    }

    public sealed class GolMarcadoLogHandler : INotificationHandler<GolMarcadoEvent>
    {
        private readonly ILogger<GolMarcadoLogHandler> _logger;
        public GolMarcadoLogHandler(ILogger<GolMarcadoLogHandler> logger) => _logger = logger;

        public Task Handle(GolMarcadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GOL! Partida {PartidaId} aos {Minuto}' - placar {Mandante} x {Visitante}",
                notification.PartidaId, notification.Minuto, notification.PlacarMandante, notification.PlacarVisitante);
            return Task.CompletedTask;
        }
    }
}
