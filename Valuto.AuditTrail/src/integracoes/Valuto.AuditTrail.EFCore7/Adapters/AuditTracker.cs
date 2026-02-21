using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EFCore7.Adapters
{
    internal class AuditTracker : IAuditTracker
    {
        private readonly ChangeTracker _changeTracker;

        public AuditTracker(ChangeTracker changeTracker)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
        }

        /// <summary>
        /// Obtém as entidades rastreadas pelo EF Core 7 e as adapta para o contrato agnóstico.
        /// </summary>
        /// <returns>Coleção de entradas filtradas por estado de modificação.</returns>
        public IEnumerable<IAuditEntry> Entries()
        {
            return _changeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .Select(e => new AuditEntryAdapter(e));
        }
    }
}
