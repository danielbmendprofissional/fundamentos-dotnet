using MassTransit;

namespace Valuto.AuditTrail.Mesaging.Interfaces
{
    internal interface IAuditBusProvider
    {
        IBusControl GetBus();
    }
}
