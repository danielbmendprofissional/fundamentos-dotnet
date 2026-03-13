using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.DTOs
{
    public sealed class PropertyMetadata
    {
        public string Name { get; }
        public bool IsPrimaryKey { get; }

        public PropertyMetadata(string name, bool isPrimaryKey)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
