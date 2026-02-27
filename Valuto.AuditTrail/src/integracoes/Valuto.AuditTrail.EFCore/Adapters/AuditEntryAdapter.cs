using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class AuditEntryAdapter : IAuditEntry
    {
        private readonly EntityEntry _entry;

        public AuditEntryAdapter(EntityEntry entry)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }
        public object Entity => _entry.Entity;

        public AuditEntryState State => MapState(_entry.State);

        public IEnumerable<IAuditProperty> Properties =>
            _entry.Properties.Select(p => new AuditPropertyAdapter(p));

        public object TypeMetadata => _entry.Metadata;

        public IAuditProperty? Property(string propertyName)
        {
            var property = _entry.Property(propertyName);
            return property != null ? new AuditPropertyAdapter(property) : null;
        }

        public object Navigation(string navigationName)
        {
            return _entry.Navigation(navigationName);
        }

        private AuditEntryState MapState(EntityState state)
        {
            return state switch
            {
                EntityState.Added => AuditEntryState.Added,
                EntityState.Modified => AuditEntryState.Modified,
                EntityState.Deleted => AuditEntryState.Deleted,
                _ => AuditEntryState.Unchanged
            };
        }
    }
}
