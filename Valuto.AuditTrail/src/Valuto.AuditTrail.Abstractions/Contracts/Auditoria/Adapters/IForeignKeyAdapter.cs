using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters
{
    public interface IForeignKeyAdapter
    {
        string Name { get; }
        bool IsTemporary { get; }
        object? CurrentValue { get; }
        string PropertyName { get; }
    }
}
