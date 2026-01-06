using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.IniciarPagamento
{
    public sealed class IniciarPagamentoCommandHandler : IRequestHandler<
        IniciarPagamentoCommand,
        IniciarPagamentoCommandResultDTO>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public IniciarPagamentoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IniciarPagamentoCommandResultDTO> Handle(IniciarPagamentoCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            // REGRA DE NEGÓCIO → DOMAIN
           var novoPagamentoId = pedido.IniciarPagamento((MetodoPagamento)request.MetodoPagamento);

            // FECHAMENTO TRANSACIONAL
            await _unitOfWork.CommitAsync(cancellationToken);

            return new IniciarPagamentoCommandResultDTO(novoPagamentoId);
        }
    }
}
