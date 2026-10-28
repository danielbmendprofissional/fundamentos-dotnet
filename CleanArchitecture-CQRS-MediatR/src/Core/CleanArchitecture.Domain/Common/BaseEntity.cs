namespace CleanArchitecture.Domain.Common
{
    /*
        abstract base entity
    */
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTimeOffset DateCreated { get; protected set; }
        public DateTimeOffset? DateUpdated { get; protected set; }
        public DateTimeOffset? DateDeleted { get; protected set; }

        protected BaseEntity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }

        public void SetDateUpdated()
        {
            DateUpdated = DateTime.UtcNow;
        }

        public void SetDateDeleted()
        {
            DateDeleted = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id != Guid.Empty
                ? Id.GetHashCode()
                : base.GetHashCode();
        }

        public static bool operator ==(BaseEntity? a, BaseEntity? b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity? a, BaseEntity? b)
        {
            return !(a == b);
        }
    }
}
