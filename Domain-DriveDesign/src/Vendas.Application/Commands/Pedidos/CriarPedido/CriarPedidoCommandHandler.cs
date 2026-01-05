using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Application.Commands.Pedidos.CriarPedido
{
    public sealed class CriarPedidoCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;

        public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<CriarPedidoResultDTO> HandleAsync(
            CriarPedidoCommand command,
            CancellationToken cancellationToken = default)
        {
            var pedido = Pedido.Criar(
                command.ClienteId,
                command.EnderecoEntrega);

            await _pedidoRepository.AdicionarAsync(pedido, cancellationToken);

            return new CriarPedidoResultDTO(
                pedido.Id,
                pedido.NumeroPedido,
                pedido.DataCriacao,
                pedido.ValorTotal,
                pedido.StatusPedido.ToString()
            );
        }
    }

}
