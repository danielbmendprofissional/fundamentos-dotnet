using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;
using Vendas.Application.Catalogo.IntegrationEvents;
using Vendas.Domain.Pedidos.Events.Pedido;

namespace Vendas.Application.Pedidos.EventHandlers
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
            //NOTIFICA A EQUIPE PARA QUE SEPARAM chamando EnviarEmailIntegrationEvent


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
