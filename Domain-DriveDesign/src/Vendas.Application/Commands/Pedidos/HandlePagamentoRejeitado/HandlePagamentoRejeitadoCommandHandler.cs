using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.HandlePagamentoAprovado;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.HandlePagamentoRejeitado
{
    public sealed class HandlePagamentoRejeitadoCommandHandler : IRequestHandler<
        HandlePagamentoRejeitadoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HandlePagamentoRejeitadoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(HandlePagamentoRejeitadoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.HandlePagamentoRejeitado(request.PagamentoId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
