using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EF
{
    internal class AuditEntryAdapter : IAuditEntry
    {
        private readonly DbEntityEntry _entry;
        public AuditEntryAdapter(DbEntityEntry entry) => _entry = entry;

        public object Entity => _entry.Entity;

        public object TypeMetadata
        {
            get
            {
                var objectContext = _entry.GetObjectContext();
                if (objectContext == null) return null;

                return objectContext.MetadataWorkspace.GetCSpaceEntityType(_entry.Entity.GetType());
            }
        }

        public IEnumerable<IAuditProperty> Properties =>
            _entry.CurrentValues.PropertyNames.Select(name => new AuditPropertyAdapter(_entry, name));

        public object Navigation(string navigationName)
        {
            try { return _entry.Member(navigationName); }
            catch { return null; }
        }

        public AuditEntryState State
        {
            get
            {
                switch (_entry.State)
                {
                    case EntityState.Added:
                        return AuditEntryState.Added;
                    case EntityState.Modified:
                        return AuditEntryState.Modified;
                    case EntityState.Deleted:
                        return AuditEntryState.Deleted;
                    default:
                        return AuditEntryState.Unchanged;
                }
            }
        }

        public IAuditProperty Property(string propertyName)
            => new AuditPropertyAdapter(_entry, propertyName);
    }
}
