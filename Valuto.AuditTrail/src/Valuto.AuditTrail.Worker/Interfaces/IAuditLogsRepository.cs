using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Interfaces
{
    public interface IAuditLogsRepository
    {
        Task AdicionarUnicoAsync(AuditLogs auditLogs, CancellationToken cancellationToken = default);
    }
}
