using Microsoft.EntityFrameworkCore;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Worker.Interfaces;

namespace Valuto.AuditTrail.Worker.Persistence.Repository
{
    internal sealed class AuditTrailsRepository : IAuditTrailsRepository
    {
        private readonly AuditDbContext _context;

        public AuditTrailsRepository(AuditDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarUnicoAsync(AuditTrails auditTrails, CancellationToken cancellationToken = default)
        {
            if (await _context.AuditTrails.AnyAsync(x => x.Id == auditTrails.Id))
                return;

            await _context.AuditTrails.AddAsync(auditTrails, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
