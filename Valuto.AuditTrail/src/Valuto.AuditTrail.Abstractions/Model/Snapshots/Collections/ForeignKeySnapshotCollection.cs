using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class ForeignKeySnapshotCollection : KeyedCollection<string, ForeignKeySnapshot>
    {
        protected override string GetKeyForItem(ForeignKeySnapshot item) => item.Name;

        public IEnumerable<ForeignKeySnapshot> AsEnumerable() => this.Items;
    }
}
