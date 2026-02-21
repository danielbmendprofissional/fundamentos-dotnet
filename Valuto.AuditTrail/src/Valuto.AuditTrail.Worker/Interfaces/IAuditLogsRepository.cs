using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Interfaces
{
    public interface IAuditLogsRepository
    {
        Task AdicionarUnicoAsync(AuditLogs auditTrails, CancellationToken cancellationToken = default);
    }
}
