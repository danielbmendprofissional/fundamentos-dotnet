using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EFCore.Adapters
{
    internal class TrackerAdapter : ITrackerAdapter
    {
        private readonly ChangeTracker _changeTracker;

        public TrackerAdapter(ChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        /// <summary>
        /// Obtém as entradas do ChangeTracker do EF Core 8 e as adapta para o contrato da Abstractions.
        /// </summary>
        /// <returns>Coleção de entradas prontas para o processamento de auditoria.</returns>
        public IEnumerable<IEntityAdapter> Entries(IMetadataAdapter metadataAdapter)
        {
            try
            {
                return _changeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .Select(e => new EntityAdapter(e, metadataAdapter));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<EntityAdapter>();
            }
        }
    }
}
