using Galo.Domain.Common.Interfaces.Events;

namespace Galo.Domain.Common.Base
{
    public abstract record class DomainEvent : IDomainEvent
    {
        public DateTime DateOccurred { get; } = DateTime.UtcNow;
    }
}
