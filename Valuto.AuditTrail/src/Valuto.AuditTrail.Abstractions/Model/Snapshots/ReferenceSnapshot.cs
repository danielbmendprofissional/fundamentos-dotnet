using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class ReferenceSnapshot
    {
        public string Name { get; } = string.Empty;
        public bool IsModified { get; }

        private ReferenceSnapshot(string name, bool isNavigationModified)
            => (Name, IsModified) = (name, isNavigationModified);

        public static ReferenceSnapshot Create(string name, bool isNavigationModified)
            => new ReferenceSnapshot(name, isNavigationModified);
    }
}
