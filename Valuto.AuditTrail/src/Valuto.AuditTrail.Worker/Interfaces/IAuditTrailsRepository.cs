
using Valuto.AuditTrail.Abstractions.Model.Entities;

namespace Valuto.AuditTrail.Worker.Interfaces
{
    public interface IAuditTrailsRepository
    {
        Task AdicionarUnicoAsync(AuditTrails auditTrails, CancellationToken cancellationToken = default);
    }
}
