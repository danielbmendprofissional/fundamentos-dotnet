using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Common.Base
{
    public abstract class Entity
    {

        public Guid Id { get; protected set; }
        public DateTime CreateDate { get; protected set; }
        public DateTime? UpdateDate { get; protected set; }

        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
        }

        protected void SetUpdateDate()
        {
            UpdateDate = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            if (ReferenceEquals(this, other)) return true;

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
