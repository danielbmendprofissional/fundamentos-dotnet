using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class NavigationSnapshotCollection : KeyedCollection<string, NavigationSnapshot>
    {
        protected override string GetKeyForItem(NavigationSnapshot item) => item.Name;

        /// <summary>
        /// Adiciona uma sequência de itens à coleção.
        /// </summary>
        /// <param name="items">Sequência de snapshots a ser adicionada.</param>
        public void AddRange(IEnumerable<NavigationSnapshot> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public IEnumerable<NavigationSnapshot> AsEnumerable() => this.Items;
    }
}
