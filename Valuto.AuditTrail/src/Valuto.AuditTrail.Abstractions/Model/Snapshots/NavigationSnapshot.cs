using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class NavigationSnapshot
    {
        public string Name { get; }
        public IReadOnlyList<object> Values { get; }

        private NavigationSnapshot(string name, IEnumerable<object> values)
        {
            Name = name;
            Values = values?.ToList() ?? new List<object>();
        }

        public static NavigationSnapshot Create(string name, IEnumerable<object> values)
            => new NavigationSnapshot(name, values);
    }
}
