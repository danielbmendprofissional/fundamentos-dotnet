using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.HandlePagamentoAprovado;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.MarcarComoEmSeparacao
{
    public sealed class MarcarComoEmSeparacaoCommandHandler : IRequestHandler<
        MarcarComoEmSeparacaoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarcarComoEmSeparacaoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MarcarComoEmSeparacaoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.MarcarComoEmSeparacao();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
