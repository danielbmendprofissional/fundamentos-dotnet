using MediatR;

namespace Galo.Application.Common.Interfaces.IntegrationEvents
{
    // Vive na camada Application (é um contrato). Herda INotification para que, ao CHEGAR do RabbitMQ,
    // o consumidor o despache localmente via MediatR para os handlers.
    public interface IIntegrationEvent : INotification
    {
        Guid Id { get; }
        DateTime DateOccurred { get; }
    }
}
