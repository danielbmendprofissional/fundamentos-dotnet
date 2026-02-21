using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EFCore3.Adapters
{
    internal class AuditTracker : IAuditTracker
    {
        private readonly ChangeTracker _changeTracker;

        public AuditTracker(ChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        /// <summary>
        /// Obtém todas as entradas rastreadas pelo EF Core e as converte para o contrato agnóstico.
        /// </summary>
        /// <returns>Uma coleção de adaptadores de entrada de auditoria.</returns>
        public IEnumerable<IAuditEntry> Entries()
        {
            return _changeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .Select(e => new AuditEntryAdapter(e));
        }
    }
}
