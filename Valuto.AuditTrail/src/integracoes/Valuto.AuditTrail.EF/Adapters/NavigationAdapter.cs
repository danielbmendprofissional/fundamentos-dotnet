using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EF.Adapters
{
    internal class NavigationAdapter : INavigationAdapter
    {
        private readonly DbMemberEntry _entry;

        public NavigationAdapter(DbMemberEntry entry, string[] namesPrimaryKeys)
        {
            _entry = entry;
            PrimaryKeys = namesPrimaryKeys ?? Array.Empty<string>();
        }

        public string Name => _entry?.Name ?? string.Empty;

        public IReadOnlyList<string> PrimaryKeys { get; }

        public IEnumerable<object> Values
        {
            get
            {
                if (_entry == null) return Enumerable.Empty<object>();

                if (_entry is DbCollectionEntry col && !col.IsLoaded)
                    return Enumerable.Empty<object>();

                var val = _entry.CurrentValue;

                if (val is IEnumerable enumerable)
                {
                    return enumerable.Cast<object>();
                }

                return Enumerable.Empty<object>();
            }
        }
    }
}
