using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Abstractions.IntegrationEvents
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync(
            object integrationEvent,
            CancellationToken cancellationToken = default);
    }
}
