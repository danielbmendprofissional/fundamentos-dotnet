using MassTransit;

namespace Valuto.AuditTrail.Mesaging.Interfaces
{
    public interface IAuditBusProvider
    {
        IBusControl GetBus();
    }
}
