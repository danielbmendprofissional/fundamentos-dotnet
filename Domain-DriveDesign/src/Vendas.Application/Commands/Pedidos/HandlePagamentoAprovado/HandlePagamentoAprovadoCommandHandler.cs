using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.CriarPedido;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.Pedidos.HandlePagamentoAprovado
{
    public sealed class HandlePagamentoAprovadoCommandHandler : IRequestHandler<
        HandlePagamentoAprovadoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HandlePagamentoAprovadoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(HandlePagamentoAprovadoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.HandlePagamentoAprovado(request.PagamentoId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
