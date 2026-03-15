using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class NavigationSnapshot
    {
        public string Name { get; }
        public IReadOnlyList<string> PrimaryKeys { get; }
        public IReadOnlyList<object> Values { get; }
        private NavigationSnapshot(string name, IReadOnlyList<string> primaryKeys, IEnumerable<object> values)
        {
            Name = name;
            PrimaryKeys = primaryKeys?.ToList() ?? new List<string>();
            Values = values as IReadOnlyList<object> ?? values?.ToList() ?? new List<object>();
        }

        public static NavigationSnapshot Create(INavigationAdapter adapter)
        {
            return new NavigationSnapshot(
                adapter.Name,
                adapter.PrimaryKeys,
                adapter.Values
            );
        }
    }
}
