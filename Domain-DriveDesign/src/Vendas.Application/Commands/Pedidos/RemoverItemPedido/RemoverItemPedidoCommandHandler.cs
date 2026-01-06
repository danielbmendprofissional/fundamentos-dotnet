using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.RemoverItemPedido
{
    public sealed class RemoverItemPedidoCommandHandler
        : IRequestHandler<RemoverItemPedidoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoverItemPedidoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            RemoverItemPedidoCommand request,
            CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            // REGRA DE NEGÓCIO → DOMAIN
            pedido.RemoverItem(request.ItemId);

            // FECHAMENTO TRANSACIONAL
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

}
