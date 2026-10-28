using MediatR;

namespace Galo.Domain.Common.Interfaces.Events
{
    /// <summary>
    /// Evento de domínio: "algo relevante aconteceu no negócio" (passado: GolMarcado, PartidaEncerrada).
    /// Herda INotification do MediatR para ser entregue a vários handlers (1 evento -> N reações).
    /// </summary>
    public interface IDomainEvent : INotification
    {
        DateTime DateOccurred { get; }
    }
}
