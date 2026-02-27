using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class AuditEntrySnapshot : IAuditEntry
    {
        public object Entity { get; set; }
        public AuditEntryState State { get; set; }
        public IEnumerable<IAuditProperty> Properties { get; set; }
        public object TypeMetadata { get; set; }

        public Dictionary<string, object> NavEntries { get; set; } = new Dictionary<string, object>();

        public IAuditProperty Property(string name) => Properties.FirstOrDefault(p => p.Name == name);

        public object Navigation(string name) => NavEntries.TryGetValue(name, out var nav) ? nav : null;
    }
}
