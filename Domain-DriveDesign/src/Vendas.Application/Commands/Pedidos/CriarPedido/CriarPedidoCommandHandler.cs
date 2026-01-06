using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.Pedidos.CriarPedido
{
    public sealed class CriarPedidoCommandHandler : IRequestHandler<
        CriarPedidoCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarPedidoCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
        {
            var endereco = EnderecoEntrega.Criar(
                request.Cep,
                request.Logradouro,
                request.Complemento ?? string.Empty,
                request.Bairro,
                request.Estado,
                request.Cidade,
                request.Pais
            );

            var pedido = Pedido.Criar(
                request.ClienteId,
                endereco);

            await _pedidoRepository.AdicionarAsync(pedido, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

}
