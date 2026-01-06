using Vendas.Domain.Common.Interfaces.Events;

namespace Vendas.Domain.Common.Base
{   // Entities are compared by identity.
    // Value Objects are compared by value.

    public abstract class Entity
    {
        /*
            Usar protected set para que apenas a própria classe ou classes
            derivadas possam alterar as propriedades.
        */

        public Guid Id { get; protected set; }
        public DateTime DataCriacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        // Construtor protegido: Entidades devem ser criadas via construtor ou factory
        // Construtor usado pelo domínio (criação real)
        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
        }

        protected void SetDataAtualizacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }

        // Sobrescrever Equals e GetHashCode é crucial para comparar entidades
        // baseado APENAS na sua identidade (Id).

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;

            // Se o ID ainda não foi persistido (é GUID.Empty), compara por referência.
            // No nosso caso incializamos no construtor, essa checagem pode ser simples
            // para apenas comparar o Id.
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id != Guid.Empty
                ? Id.GetHashCode()
                : base.GetHashCode();
        }

        public static bool operator ==(Entity? a, Entity? b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(Entity? a, Entity? b)
        {
            return !(a == b);
        }

    }
}
