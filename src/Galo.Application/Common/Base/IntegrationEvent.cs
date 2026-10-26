using Galo.Application.Common.Interfaces.IntegrationEvents;

namespace Galo.Application.Common.Base
{
    /// <summary>
    /// Evento de INTEGRAÇÃO: contrato de saída/entrada pela mensageria (RabbitMQ).
    /// Diferente do evento de DOMÍNIO (interno ao processo, ligado ao agregado), este é serializado em JSON,
    /// precisa ser estável/versionável e pode ser consumido por outro serviço.
    /// </summary>
    public abstract record class IntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime DateOccurred { get; init; } = DateTime.UtcNow;
    }
}
