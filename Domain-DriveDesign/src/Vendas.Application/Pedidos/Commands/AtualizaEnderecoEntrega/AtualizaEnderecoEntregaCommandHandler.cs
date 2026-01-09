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

namespace Vendas.Application.Pedidos.Commands.AtualizaEnderecoEntrega
{
    public sealed class AtualizaEnderecoEntregaCommandHandler : IRequestHandler<
        AtualizarEnderecoEntregaCommand>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AtualizaEnderecoEntregaCommandHandler(
            IPedidoRepository pedidoRepository,
            IUnitOfWork unitOfWork)
        {
            _pedidoRepository = pedidoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AtualizarEnderecoEntregaCommand request, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            var endereco = EnderecoEntrega.Criar(
                request.Endereco.Cep,
                request.Endereco.Logradouro,
                request.Endereco.Complemento ?? string.Empty,
                request.Endereco.Bairro,
                request.Endereco.Estado,
                request.Endereco.Cidade,
                request.Endereco.Pais
            );

            pedido.AtualizarEnderecoEntrega(endereco);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
