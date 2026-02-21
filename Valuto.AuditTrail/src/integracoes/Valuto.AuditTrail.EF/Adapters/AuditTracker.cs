using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;

namespace Valuto.AuditTrail.EF
{
    internal class AuditTracker : IAuditTracker
    {
        private readonly DbContext _context;
        public AuditTracker(DbContext context) => _context = context;

        public IEnumerable<IAuditEntry> Entries()
        {
            return _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .Select(e => new AuditEntryAdapter(e));
        }
    }
}
