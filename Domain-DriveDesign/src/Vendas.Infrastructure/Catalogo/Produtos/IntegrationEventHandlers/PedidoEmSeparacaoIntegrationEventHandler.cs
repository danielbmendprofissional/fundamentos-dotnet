using MediatR;
using Vendas.Application.Catalogo.Produtos.Commands.ProdutoAjustarEstoque;
using Vendas.Application.Catalogo.Produtos.IntegrationEvents;

namespace Vendas.Infrastructure.Catalogo.Produtos.IntegrationEventHandlers
{
    public sealed class PedidoEmSeparacaoIntegrationEventHandler
    {
        private readonly IMediator _mediator;

        public PedidoEmSeparacaoIntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(
            PedidoEmSeparacaoIntegrationEvent evt,
            CancellationToken cancellationToken)
        {
            //NOTIFICA A EQUIPE DE ESTOQUE PARA SEPARAR --- TAMBÉM

            var command = new ProdutoAjustarEstoqueCommand(evt.PedidoId, evt.Produtos);
            await _mediator.Send(command, cancellationToken);
        }
    }

}
