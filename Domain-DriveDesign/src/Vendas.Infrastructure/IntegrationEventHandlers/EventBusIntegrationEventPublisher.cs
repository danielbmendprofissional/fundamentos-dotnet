using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.IntegrationEvents;

namespace Vendas.Infrastructure.IntegrationEventHandlers
{
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
