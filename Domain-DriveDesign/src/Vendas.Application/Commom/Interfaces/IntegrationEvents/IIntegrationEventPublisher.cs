
using Vendas.Application.Commom.Base;

namespace Vendas.Application.Commom.Interfaces.IntegrationEvents
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync<T>(
            T integrationEvent,
            CancellationToken cancellationToken = default) where T : IntegrationEvent;
    }
}
