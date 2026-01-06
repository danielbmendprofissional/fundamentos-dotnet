using Vendas.Domain.Common.Interfaces.Events;

namespace Vendas.Domain.Common.Base
{
    public abstract class AggregateRoot : Entity
    {

        //permitir que entidades roots do domínio registrem internamente os eventos para que
        //sejam processados mais tarde, geralmete após SaveChanges();

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() : base()
        {
        }

        protected AggregateRoot(Guid id) : base(id)
        {
        }

        //Add é protected because only root pode adicionar o evento, remover e limpar
        //normalmente quem faz são outras camadas.
        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void RemoveDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Remove(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
