using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EF
{
    internal class TrackerAdapter : ITrackerAdapter
    {
        private readonly DbContext _context;
        public TrackerAdapter(DbContext context) => _context = context;

        public IEnumerable<IEntityAdapter> Entries(IMetadataAdapter metadataAdapter)
        {
            try
            {
                return _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .Select(e => new EntityAdapter(_context, e, metadataAdapter));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<EntityAdapter>();
            }
        }
    }
}
