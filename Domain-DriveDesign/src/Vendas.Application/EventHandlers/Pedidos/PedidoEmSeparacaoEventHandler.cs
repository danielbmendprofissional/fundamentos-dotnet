using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.IntegrationEvents;
using Vendas.Application.IntegrationEvents.Catalogo;
using Vendas.Domain.Pedidos.Events.Pedido;

namespace Vendas.Application.EventHandlers.Pedidos
{
    public sealed class PedidoEmSeparacaoEventHandler
        : INotificationHandler<PedidoEmSeparacaoEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;

        public PedidoEmSeparacaoEventHandler(
            IIntegrationEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(
            PedidoEmSeparacaoEvent notification,
            CancellationToken cancellationToken)
        {

            var integrationEvent =
                new PedidoEmSeparacaoIntegrationEvent(
                    notification.PedidoId,
                    notification.Itens.Select(i =>
                        new ProdutoReservaDTO(
                            i.ProdutoId,
                            i.Quantidade
                        )).ToList()
                );

            await _publisher.PublishAsync(
                integrationEvent,
                cancellationToken);
        }
    }

}
