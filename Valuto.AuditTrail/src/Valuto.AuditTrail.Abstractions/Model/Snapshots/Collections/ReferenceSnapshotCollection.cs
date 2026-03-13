using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class ReferenceSnapshotCollection : KeyedCollection<string, ReferenceSnapshot>
    {
        protected override string GetKeyForItem(ReferenceSnapshot item) => item.Name;
    }
}
