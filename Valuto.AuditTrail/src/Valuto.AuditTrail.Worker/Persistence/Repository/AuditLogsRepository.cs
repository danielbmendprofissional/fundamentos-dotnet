using Microsoft.EntityFrameworkCore;
using Valuto.AuditTrail.Worker.Interfaces;
using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Persistence.Repository
{
    internal sealed class AuditLogsRepository : IAuditLogsRepository
    {
        private readonly AuditDbContext _context;

        public AuditLogsRepository(AuditDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarUnicoAsync(AuditLogs auditTrails, CancellationToken cancellationToken = default)
        {
            if (await _context.AuditLogs.AnyAsync(x => x.Id == auditTrails.Id))
                return;

            await _context.AuditLogs.AddAsync(auditTrails, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
