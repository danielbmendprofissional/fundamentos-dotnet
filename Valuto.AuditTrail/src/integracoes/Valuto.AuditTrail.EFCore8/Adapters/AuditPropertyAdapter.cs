using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EFCore8.Adapters
{
    internal class AuditPropertyAdapter : IAuditProperty
    {
        private readonly PropertyEntry _property;

        public AuditPropertyAdapter(PropertyEntry property)
        {
            _property = property;
        }

        public string Name => _property.Metadata.Name;

        public object CurrentValue => _property.CurrentValue;

        public object OriginalValue => _property.OriginalValue;

        public bool IsModified => _property.IsModified;

        public bool IsTemporary => _property.IsTemporary;

        public object Metadata => _property.Metadata;
    }
}
