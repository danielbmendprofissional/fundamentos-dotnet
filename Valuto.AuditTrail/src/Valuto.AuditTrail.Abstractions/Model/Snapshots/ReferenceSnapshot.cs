using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class ReferenceSnapshot
    {
        public string Name { get; } = string.Empty;
        public bool IsModified { get; }

        private ReferenceSnapshot(string name, bool isModified)
            => (Name, IsModified) = (name, isModified);

        public static ReferenceSnapshot Create(IReferenceAdapter adapter)
        {
            return new ReferenceSnapshot(
                    adapter.Name,
                    adapter.IsModified
                );
        }
    }
}
