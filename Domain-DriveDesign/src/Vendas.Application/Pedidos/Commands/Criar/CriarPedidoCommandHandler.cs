using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Pedidos.Commands.Criar
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
                request.Endereco.Cep,
                request.Endereco.Logradouro,
                request.Endereco.Complemento ?? string.Empty,
                request.Endereco.Bairro,
                request.Endereco.Estado,
                request.Endereco.Cidade,
                request.Endereco.Pais
            );

            var pedido = Pedido.Criar(
                request.ClienteId,
                endereco);

            await _pedidoRepository.AdicionarAsync(pedido, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

}
