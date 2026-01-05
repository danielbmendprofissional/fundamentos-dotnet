using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoCommand : IRequest<AdicionarItemAoPedidoResultDTO>
    {
        public Guid PedidoId { get; }
        public Guid ProdutoId { get; }
        public string NomeProduto { get; }
        public decimal PrecoUnitario { get; }
        public int Quantidade { get; }

        public AdicionarItemAoPedidoCommand(
            Guid pedidoId,
            Guid produtoId,
            string nomeProduto,
            decimal precoUnitario,
            int quantidade)
        {
            PedidoId = pedidoId;
            ProdutoId = produtoId;
            NomeProduto = nomeProduto;
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
        }
    }

}
