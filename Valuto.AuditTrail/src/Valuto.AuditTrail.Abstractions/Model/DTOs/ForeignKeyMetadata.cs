using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.DTOs
{
    public sealed class ForeignKeyMetadata
    {
        public string Name { get; }
        public string PropertyName { get; }

        public ForeignKeyMetadata(string name, string propertyName)
        {
            Name = name;
            PropertyName = propertyName;
        }
    }
}
