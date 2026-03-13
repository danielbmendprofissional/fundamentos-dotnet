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
        private readonly bool _isCollection;

        public NavigationAdapter(DbMemberEntry entry, bool isCollection)
        {
            _entry = entry;
            _isCollection = isCollection;
        }

        public string Name => _entry.Name;

        public IEnumerable<object> Values
        {
            get
            {
                if (_isCollection)
                {
                    if (_entry is DbCollectionEntry col && !col.IsLoaded)
                        return Enumerable.Empty<object>();
                }
                else
                {
                    if (_entry is DbReferenceEntry refEntry && !refEntry.IsLoaded)
                        return Enumerable.Empty<object>();
                }

                var val = _entry.CurrentValue;
                if (val == null) return Enumerable.Empty<object>();

                if (_isCollection && val is IEnumerable enumerable)
                {
                    return enumerable.Cast<object>();
                }

                return new[] { val };
            }
        }
    }
}
