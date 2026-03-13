using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class PropertySnapshotCollection : KeyedCollection<string, PropertySnapshot>
    {
        protected override string GetKeyForItem(PropertySnapshot item) => item.Name;

        public IEnumerable<PropertySnapshot> AsEnumerable() => this.Items;
    }
}
