using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;

        public AdicionarItemAoPedidoCommandHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<AdicionarItemAoPedidoResultDTO> HandleAsync(
            AdicionarItemAoPedidoCommand command, CancellationToken cancellationToken = default)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(command.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.AdicionarItem(command.ProdutoId, command.NomeProduto, command.PrecoUnitario, command.Quantidade);

            await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);

            return new AdicionarItemAoPedidoResultDTO(
                pedido.Id,
                pedido.ValorTotal,
                pedido.StatusPedido.ToString()
            );
        }
    }
}
