using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters
{
    public interface IReferenceAdapter
    {
        string Name { get; }
        bool IsModified { get; }
    }
}
