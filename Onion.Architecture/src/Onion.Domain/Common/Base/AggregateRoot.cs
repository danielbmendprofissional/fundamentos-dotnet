using Onion.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Common.Base
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(Guid id) : base(id)
        {
        }
        
        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void RemoveDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Remove(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
