using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class NavigationAdapter : INavigationAdapter
    {
        private readonly NavigationEntry _navigation;

        public NavigationAdapter(NavigationEntry entry, string[] namesPrimaryKeys)
        {
            _navigation = entry;
            NamesPrimaryKeys = namesPrimaryKeys;
        }

        public string Name => _navigation.Metadata.Name;

        public string[] NamesPrimaryKeys { get; }

        public IEnumerable<object> Values
        {
            get
            {
                var val = _navigation.CurrentValue as IEnumerable;
                return val?.Cast<object>() ?? Enumerable.Empty<object>();
            }
        }
    }
}
