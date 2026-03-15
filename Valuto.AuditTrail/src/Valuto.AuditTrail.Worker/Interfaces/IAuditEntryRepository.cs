
using Valuto.AuditTrail.Abstractions.Model.Entities;

namespace Valuto.AuditTrail.Worker.Interfaces
{
    public interface IAuditEntryRepository
    {
        Task AdicionarUnicoAsync(AuditEntry auditEntry, CancellationToken cancellationToken = default);
    }
}
