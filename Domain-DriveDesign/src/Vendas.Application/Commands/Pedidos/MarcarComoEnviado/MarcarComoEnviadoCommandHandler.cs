using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.MarcarComoEntregue;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.MarcarComoEnviado
{
    public sealed class MarcarComoEnviadoCommandHandler : IRequestHandler<
            MarcarComoEnviadoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarcarComoEnviadoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MarcarComoEnviadoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.MarcarComoEnviado();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
