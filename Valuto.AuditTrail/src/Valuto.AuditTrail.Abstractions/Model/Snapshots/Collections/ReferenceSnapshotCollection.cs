using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class ReferenceSnapshotCollection : KeyedCollection<string, ReferenceSnapshot>
    {
        protected override string GetKeyForItem(ReferenceSnapshot item) => item.Name;

        /// <summary>
        /// Adiciona uma sequência de itens à coleção.
        /// </summary>
        /// <param name="items">Sequência de snapshots a ser adicionada.</param>
        public void AddRange(IEnumerable<ReferenceSnapshot> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public IEnumerable<ReferenceSnapshot> AsEnumerable() => this.Items;
    }
}
