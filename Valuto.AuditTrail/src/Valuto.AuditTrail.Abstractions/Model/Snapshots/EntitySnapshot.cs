using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    /// <summary>
    /// Representa um snapshot imutável do estado de uma entidade rastreada para fins de auditoria.
    /// </summary>
    public sealed class EntitySnapshot
    {
        public string Name { get; }
        public string Namespace { get; }

        public EntryState State { get; }

        public NavigationSnapshotCollection NavEntries { get; } = new NavigationSnapshotCollection();
        public ReferenceSnapshotCollection References { get; } = new ReferenceSnapshotCollection();
        public PropertySnapshotCollection Properties { get; } = new PropertySnapshotCollection();
        public ForeignKeySnapshotCollection ForeignKeys { get; } = new ForeignKeySnapshotCollection();

        private EntitySnapshot(string name, string @namespace, EntryState state)
            => (Name, Namespace, State) = (name, @namespace, state);

        public static EntitySnapshot Create(string name, string? @namespace, EntryState state)
            => new EntitySnapshot(name, @namespace ?? string.Empty, state);
    }
}
