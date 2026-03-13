using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class NavigationSnapshotCollection : KeyedCollection<string, NavigationSnapshot>
    {
        protected override string GetKeyForItem(NavigationSnapshot item) => item.Name;

        public NavigationSnapshot? Get(string name)
            => Contains(name) ? this[name] : null;

        public IEnumerable<NavigationSnapshot> AsEnumerable() => this.Items;
    }
}
