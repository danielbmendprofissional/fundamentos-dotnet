using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.EF.Adapters
{
    internal class ForeignKeyAdapter : IForeignKeyAdapter
    {
        private readonly DbPropertyEntry _property;
        private readonly bool _isTemporary;

        public string Name { get; }
        public string PrincipalEntityName { get; }
        public string PropertyName => _property.Name;
        public object CurrentValue => _property.CurrentValue;
        public bool IsTemporary => _isTemporary;

        public ForeignKeyAdapter(
            DbPropertyEntry property,
            string resolvedName,
            IMetadataAdapter metadataAdapter,
            object edmMetadata,
            bool isTemporary)
        {
            _property = property;
            Name = resolvedName;
            _isTemporary = isTemporary;
            PrincipalEntityName = metadataAdapter.GetPrincipalEntityName(edmMetadata);
        }
    }
}
