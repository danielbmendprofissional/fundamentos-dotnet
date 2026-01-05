using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoResultDTO
    {
        public Guid Pedidold { get; }
        public decimal ValorTotal { get; }
        public string Status { get; }

        public AdicionarItemAoPedidoResultDTO(
            Guid pedidold,
            decimal valorTotal,
            string status)
        {
            Pedidold = pedidold;
            ValorTotal = valorTotal;
            Status = status;
        }
    }
}
