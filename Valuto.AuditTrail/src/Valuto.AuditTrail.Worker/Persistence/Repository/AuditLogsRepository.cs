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

        public async Task AdicionarUnicoAsync(AuditLogs auditLogs, CancellationToken cancellationToken = default)
        {
            if (await _context.AuditLogs.AnyAsync(x => x.Id == auditLogs.Id))
                return;

            await _context.AuditLogs.AddAsync(auditLogs, cancellationToken);
            await _context.SaveChangesAsync();
        }
    }
}
