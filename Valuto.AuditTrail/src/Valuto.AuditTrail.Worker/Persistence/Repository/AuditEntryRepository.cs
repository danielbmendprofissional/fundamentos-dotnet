using Microsoft.EntityFrameworkCore;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Worker.Interfaces;

namespace Valuto.AuditTrail.Worker.Persistence.Repository
{
    internal sealed class AuditEntryRepository : IAuditEntryRepository
    {
        private readonly AuditDbContext _context;

        public AuditEntryRepository(AuditDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarUnicoAsync(AuditEntry auditEntry, CancellationToken cancellationToken = default)
        {
            if (await _context.AuditEntry.AnyAsync(x => x.Id == auditEntry.Id))
                return;

            await _context.AuditEntry.AddAsync(auditEntry, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
