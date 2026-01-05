namespace Vendas.Domain.Common.Interfaces.Events
{
    public interface IDomainEvent
    {
        DateTime DateOccurred { get; }
    }
}
