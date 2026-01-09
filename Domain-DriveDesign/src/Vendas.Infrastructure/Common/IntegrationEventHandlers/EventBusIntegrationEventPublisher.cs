using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;

namespace Vendas.Infrastructure.Common.IntegrationEventHandlers
{
    //IDEAL aplicar OUTBOX
    public sealed class EventBusIntegrationEventPublisher
        : IIntegrationEventPublisher
    {
        public async Task PublishAsync(
            object integrationEvent,
            CancellationToken cancellationToken = default)
        {
            // Aqui entra RabbitMQ, Azure Service Bus, Kafka, etc.
            await Task.CompletedTask;
        }
    }
}
