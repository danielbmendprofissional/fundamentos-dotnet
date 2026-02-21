using MassTransit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Mesaging.Interfaces;
#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1
using GreenPipes;
#endif

namespace Valuto.AuditTrail.Mesaging
{
    internal class BusService<T> : IBusService<T> where T : class
    {
        private readonly IBusControl _bus;

        public BusService(IAuditBusProvider busProvider)
        {
            _bus = busProvider.GetBus();
        }

        public async Task PublishAsync(IEnumerable<T> messages, CancellationToken ct = default)
        {
#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1
            foreach (var message in messages)
            {
                await _bus.Publish(message, ct);
            }
#else
        await _bus.PublishBatch(messages, ct);
#endif
        }
    }
}
