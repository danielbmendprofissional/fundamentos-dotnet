using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class PropertyAdapter : IPropertyAdapter
    {
        private readonly PropertyEntry _property;

        public PropertyAdapter(PropertyEntry property, bool isPrimaryKey)
        {
            _property = property;
            IsPrimaryKey = isPrimaryKey;
        }

        public string Name => _property?.Metadata.Name ?? string.Empty;
        public object? CurrentValue => _property?.CurrentValue;
        public object? OriginalValue => _property?.OriginalValue;
        public bool IsModified => _property?.IsModified ?? false;
        public bool IsTemporary => _property?.IsTemporary ?? false;
        public bool IsPrimaryKey { get; }
    }
}
