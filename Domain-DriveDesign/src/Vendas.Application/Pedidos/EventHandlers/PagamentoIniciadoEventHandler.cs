using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;
using Vendas.Application.Catalogo.IntegrationEvents;
using Vendas.Domain.Pedidos.Events.Pagamento;

namespace Vendas.Application.Pedidos.EventHandlers
{
    public sealed class PagamentoIniciadoEventHandler
            : INotificationHandler<PagamentoIniciadoEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;

        public PagamentoIniciadoEventHandler(IIntegrationEventPublisher publisher)
        {
            _publisher = publisher;
        }


        public async Task Handle(
            PagamentoIniciadoEvent notification,
            CancellationToken cancellationToken)
        {
            // NOTIFICA O CLIENTE CHAMANDO EnviarEmailIntegrationEvent

            var integrationEvent =
                new PagamentoIniciadoIntegrationEvent(
                    notification.PagamentoId,
                    notification.Valor,
                    notification.CodigoTransacao
                );

            await _publisher.PublishAsync(
                integrationEvent,
                cancellationToken);
        }
    }
}
