using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class ForeignKeyAdapter : IForeignKeyAdapter
    {
        private readonly PropertyEntry _property;

        public string Name { get; }

        public bool IsTemporary => _property?.IsTemporary ?? false;
        public object? CurrentValue => _property?.CurrentValue;
        public string PropertyName => _property?.Metadata.Name ?? string.Empty;

        public ForeignKeyAdapter(PropertyEntry property, string resolvedName)
        {
            _property = property;
            Name = resolvedName;
        }
    }
}
