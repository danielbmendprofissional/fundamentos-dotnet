using System;
using System.Collections.Generic;
using System.Text;

namespace Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters
{
    public interface INavigationAdapter
    {
        string Name { get; }
        IReadOnlyList<string> PrimaryKeys { get; }
        IEnumerable<object> Values { get; }
    }
}
