using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class AuditTracker : IAuditTracker
    {
        private readonly ChangeTracker _changeTracker;

        public AuditTracker(ChangeTracker changeTracker)
        {
            _changeTracker = changeTracker ?? throw new ArgumentNullException(nameof(changeTracker));
        }

        /// <summary>
        /// Obtém as entradas do ChangeTracker do EF Core 8 e as adapta para o contrato da Abstractions.
        /// </summary>
        /// <returns>Coleção de entradas prontas para o processamento de auditoria.</returns>
        public IEnumerable<IAuditEntry> Entries()
        {
            return _changeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .Select(e => new AuditEntryAdapter(e));
        }
    }
}
