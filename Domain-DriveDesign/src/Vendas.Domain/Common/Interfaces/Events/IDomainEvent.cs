using MediatR;

namespace Vendas.Domain.Common.Interfaces.Events
{
    public interface IDomainEvent : INotification
    {
        DateTime DateOccurred { get; }
    }
}
