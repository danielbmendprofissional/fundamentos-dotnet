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
    internal class AuditPropertyAdapter : IAuditProperty
    {
        private readonly DbEntityEntry _entry;
        private readonly string _propertyName;

        public AuditPropertyAdapter(DbEntityEntry entry, string propertyName)
        {
            _entry = entry;
            _propertyName = propertyName;
        }

        public string Name => _propertyName;
        public object CurrentValue => _entry.CurrentValues[_propertyName];
        public object OriginalValue
        {
            get
            {
                if (_entry.State == EntityState.Added)
                    return null;

                return _entry.OriginalValues[_propertyName];
            }
        }
        public bool IsModified => _entry.Property(_propertyName).IsModified;
        public bool IsTemporary => false;
        public object Metadata => _entry.Property(_propertyName);
    }
}
