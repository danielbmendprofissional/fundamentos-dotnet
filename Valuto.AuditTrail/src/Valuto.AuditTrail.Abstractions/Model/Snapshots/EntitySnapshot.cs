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

        public NavigationSnapshotCollection NavEntries { get; }
        public ReferenceSnapshotCollection References { get; }
        public PropertySnapshotCollection Properties { get; }
        public ForeignKeySnapshotCollection ForeignKeys { get; }

        private EntitySnapshot(string name, string @namespace, EntryState state)
        {
            Name = name;
            Namespace = @namespace;
            State = state;

            NavEntries = new NavigationSnapshotCollection();
            References = new ReferenceSnapshotCollection();
            Properties = new PropertySnapshotCollection();
            ForeignKeys = new ForeignKeySnapshotCollection();
        }

        public static EntitySnapshot Create(string name, string? @namespace, EntryState state)
            => new EntitySnapshot(name, @namespace ?? string.Empty, state);
    }
}
