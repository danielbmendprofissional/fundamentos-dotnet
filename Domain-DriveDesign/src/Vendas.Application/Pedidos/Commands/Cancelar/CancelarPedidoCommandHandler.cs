using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Pedidos.Commands.Cancelar
{
    public sealed class CancelarPedidoCommandHandler
        : IRequestHandler<CancelarPedidoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelarPedidoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            CancelarPedidoCommand request,
            CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository
                .ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            // Application cria o Value Object
            MotivoCancelamento? motivo = request.CodigoMotivo switch
            {
                null => null,
                "ClienteDesistiu" => MotivoCancelamento.ClienteDesistiu(),
                "ErroPagamento" => MotivoCancelamento.ErroPagamento(),
                "ItemSemEstoque" => MotivoCancelamento.ItemSemEstoque(),
                "EnderecoInvalido" => MotivoCancelamento.EnderecoInvalido(),
                _ => MotivoCancelamento.Outro()
            };

            // Regra de negócio no domínio
            pedido.CancelarPedido(motivo);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
