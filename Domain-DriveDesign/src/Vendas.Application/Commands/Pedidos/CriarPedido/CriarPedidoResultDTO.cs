using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.CriarPedido
{
    public sealed class CriarPedidoResultDTO
    {
        public Guid PedidoId { get; }
        public string NumeroPedido { get; }
        public DateTime DataCriacao { get; }
        public decimal ValorTotal { get; }
        public string Status { get; }

        public CriarPedidoResultDTO(
            Guid pedidoId,
            string numeroPedido,
            DateTime dataCriacao,
            decimal valorTotal,
            string status)
        {
            PedidoId = pedidoId;
            NumeroPedido = numeroPedido;
            DataCriacao = dataCriacao;
            ValorTotal = valorTotal;
            Status = status;
        }
    }
}
