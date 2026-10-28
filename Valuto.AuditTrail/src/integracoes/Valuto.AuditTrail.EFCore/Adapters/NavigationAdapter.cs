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
        private readonly IReadOnlyList<string> _primaryKeys;

        public NavigationAdapter(NavigationEntry entry, string[] primaryKeys)
        {
            _navigation = entry;
            _primaryKeys = primaryKeys?.ToList() ?? new List<string>();
        }

        public string Name => _navigation?.Metadata?.Name ?? string.Empty;

        public IReadOnlyList<string> PrimaryKeys => _primaryKeys;

        public IEnumerable<object> Values
        {
            get
            {
                if (_navigation == null) return Enumerable.Empty<object>();

                var val = _navigation.CurrentValue as IEnumerable;
                return (val is string) ? Enumerable.Empty<object>() : (val?.Cast<object>() ?? Enumerable.Empty<object>());
            }
        }
    }
}
