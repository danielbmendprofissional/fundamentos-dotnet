using Galo.Application.Common.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Galo.Infrastructure.Messaging
{
    /// <summary>
    /// Consumidor do evento "EnviarEmail". Aqui só registramos em log (não há SMTP no ambiente de estudo).
    /// Ponto de extensão: injetar um IEmailService (anti-corruption layer) e enviar de verdade.
    /// </summary>
    public sealed class EnviarEmailIntegrationEventHandler : INotificationHandler<EnviarEmailIntegrationEvent>
    {
        private readonly ILogger<EnviarEmailIntegrationEventHandler> _logger;
        public EnviarEmailIntegrationEventHandler(ILogger<EnviarEmailIntegrationEventHandler> logger) => _logger = logger;

        public Task Handle(EnviarEmailIntegrationEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[E-MAIL simulado] para {Destinatario}: {Assunto} (evento {EventId})",
                notification.Destinatario, notification.Assunto, notification.Id);
            return Task.CompletedTask;
        }
    }
}
