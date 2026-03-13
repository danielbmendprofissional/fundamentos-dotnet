using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EF
{
    internal class PropertyAdapter : IPropertyAdapter
    {
        private readonly DbEntityEntry _entry;
        private readonly string _propertyName;
        private readonly bool _isPrimaryKey;

        public PropertyAdapter(DbEntityEntry entry, string propertyName, bool isPrimaryKey)
        {
            _entry = entry;
            _propertyName = propertyName;
            _isPrimaryKey = isPrimaryKey;
        }

        public string Name => _propertyName;

        public object CurrentValue
        {
            get
            {
                if (_entry.State == EntityState.Deleted) return null;

                return _entry.CurrentValues[_propertyName];
            }
        }

        public object OriginalValue
        {
            get
            {
                if (_entry.State == EntityState.Added) return null;

                return _entry.OriginalValues[_propertyName];
            }
        }

        public bool IsModified => _entry.Property(_propertyName).IsModified;

        public bool IsTemporary => false;

        public bool IsPrimaryKey => _isPrimaryKey;
    }
}
