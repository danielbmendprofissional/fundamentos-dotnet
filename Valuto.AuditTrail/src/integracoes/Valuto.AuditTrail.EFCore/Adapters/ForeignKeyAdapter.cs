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
        public string PrincipalEntityName { get; }

        public bool IsTemporary => _property.IsTemporary;
        public object? CurrentValue => _property.CurrentValue;
        public string PropertyName => _property.Metadata.Name;

        public ForeignKeyAdapter(PropertyEntry property, string resolvedName, IMetadataAdapter metadataAdapter)
        {
            _property = property;
            Name = resolvedName;

            PrincipalEntityName = metadataAdapter.GetPrincipalEntityName(property.Metadata);
        }
    }
}
