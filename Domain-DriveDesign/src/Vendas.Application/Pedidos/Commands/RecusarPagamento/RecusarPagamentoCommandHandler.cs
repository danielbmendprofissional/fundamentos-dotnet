using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Pedidos.Commands.RecusarPagamento
{
    public sealed class RecusarPagamentoCommandHandler
            : IRequestHandler<RecusarPagamentoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecusarPagamentoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            RecusarPagamentoCommand request,
            CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.RecusarPagamento(request.PagamentoId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
