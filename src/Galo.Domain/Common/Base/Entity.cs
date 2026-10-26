namespace Galo.Domain.Common.Base
{
    // Entidades são comparadas pela IDENTIDADE (Id).
    // Value Objects são comparados pelo VALOR.
    public abstract class Entity
    {
        // protected set: só a própria classe (ou filhas) altera o estado => domínio não anêmico.
        public Guid Id { get; protected set; }
        public DateTime DataCriacao { get; protected set; }
        public DateTime? DataAtualizacao { get; protected set; }

        // Construtor protegido. Também serve de "construtor vazio" para o EF Core
        // materializar a entidade (as filhas têm um private ctor() que chama este).
        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            DataCriacao = DateTime.UtcNow;
        }

        protected void SetDataAtualizacao() => DataAtualizacao = DateTime.UtcNow;

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Entity? a, Entity? b)
            => a is null ? b is null : a.Equals(b);

        public static bool operator !=(Entity? a, Entity? b) => !(a == b);
    }
}
