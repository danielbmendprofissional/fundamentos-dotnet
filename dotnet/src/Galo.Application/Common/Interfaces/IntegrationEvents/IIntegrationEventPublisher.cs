using Galo.Application.Common.Base;

namespace Galo.Application.Common.Interfaces.IntegrationEvents
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    }
}
