using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.ConfirmarPagamento
{
    public sealed class ConfirmarPagamentoCommandHandler
            : IRequestHandler<ConfirmarPagamentoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmarPagamentoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ConfirmarPagamentoCommand request,
            CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.ConfirmarPagamento(request.PagamentoId);
            pedido.MarcarComoEmSeparacao();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
