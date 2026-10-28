using Galo.Domain.Common.Interfaces.Events;

namespace Galo.Domain.Common.Base
{
    /// <summary>
    /// Raiz de agregado: único ponto de entrada para alterar o agregado e única que
    /// registra eventos de domínio. Os eventos ficam em memória e são despachados
    /// pelo DbContext DEPOIS do SaveChanges (ver GaloDbContext).
    /// Só raízes têm repositório.
    /// </summary>
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected AggregateRoot() : base() { }
        protected AggregateRoot(Guid id) : base(id) { }

        // protected: somente a raiz adiciona; quem despacha (infra) limpa.
        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
