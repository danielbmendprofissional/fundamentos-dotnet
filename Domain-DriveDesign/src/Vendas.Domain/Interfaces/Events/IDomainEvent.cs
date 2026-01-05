namespace Vendas.Domain.Interfaces.Events
{
    public interface IDomainEvent
    {
        DateTime DateOccurred { get; }
    }
}
