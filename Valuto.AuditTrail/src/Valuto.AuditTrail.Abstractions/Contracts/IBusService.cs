using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Model.Events;

namespace Valuto.AuditTrail.Abstractions.Contracts
{
    /// <summary>
    /// Interface para realizar operações no rabbitMQ
    /// </summary>
    public interface IBusService<T> where T : class
    {
        /// <summary>
        /// Publica em lote os eventos no RabbitMQ.
        /// </summary>
        /// <param name="messages">Coleção de eventos a serem publicados. A coleção é iterada apenas no momento do envio.</param>
        /// <param name="cancellationToken">Token de cancelamento para interromper a operação assíncrona.</param>
        Task PublishAsync(IEnumerable<T> messages, CancellationToken cancellationToken = default);
    }

}
