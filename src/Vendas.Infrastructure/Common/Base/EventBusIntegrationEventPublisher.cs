using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Base;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;

namespace Vendas.Infrastructure.Common.Base
{
    //IDEAL aplicar OUTBOX
    public sealed class EventBusIntegrationEventPublisher : IIntegrationEventPublisher
    {
        private readonly ILogger<EventBusIntegrationEventPublisher> _logger;
        // No futuro, aqui você injetaria o IPublishEndpoint (MassTransit) ou IModel (RabbitMQ)

        public EventBusIntegrationEventPublisher(ILogger<EventBusIntegrationEventPublisher> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(
            T integrationEvent,
            CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            try
            {
                _logger.LogInformation("Publicando evento de integração: {EventId} às {Date}",
                    integrationEvent.Id, integrationEvent.DateOccurred);

                // 1. Serialização (Simulação)
                // var message = JsonSerializer.Serialize(integrationEvent);

                // 2. Envio Real (Exemplo conceitual com RabbitMQ/Bus)
                // await _bus.Publish(integrationEvent, cancellationToken);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao publicar evento {EventId}", integrationEvent.Id);
                // Aqui você decide se relança a exceção ou se usa uma estratégia de Retry
                throw;
            }
        }
    }
}
