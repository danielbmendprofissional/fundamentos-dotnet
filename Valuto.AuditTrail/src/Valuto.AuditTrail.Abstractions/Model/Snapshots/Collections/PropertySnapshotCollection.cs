using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots.Collections
{
    public class PropertySnapshotCollection : KeyedCollection<string, PropertySnapshot>
    {
        protected override string GetKeyForItem(PropertySnapshot item) => item.Name;

        /// <summary>
        /// Adiciona uma sequência de itens à coleção.
        /// </summary>
        /// <param name="items">Sequência de snapshots a ser adicionada.</param>
        public void AddRange(IEnumerable<PropertySnapshot> items)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Expõe os itens da coleção como uma sequência somente leitura.
        /// </summary>
        public IEnumerable<PropertySnapshot> AsEnumerable() => this.Items;
    }
}

