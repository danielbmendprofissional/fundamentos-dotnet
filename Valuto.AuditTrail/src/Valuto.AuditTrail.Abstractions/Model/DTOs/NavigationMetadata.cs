using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Model.DTOs
{
    public sealed class NavigationMetadata
    {
        public string Name { get; }
        public bool IsCollection { get; }
        public string[] PrimaryKeys { get; }

        public NavigationMetadata(string name, bool isCollection, string[] primaryKeys)
        {
            Name = name;
            IsCollection = isCollection;
            PrimaryKeys = primaryKeys;
        }
    }
}
