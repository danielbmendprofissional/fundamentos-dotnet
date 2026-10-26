using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Commands.AdicionarItem
{
    public sealed record AdicionarItemAoPedidoResultDTO(
        Guid PedidoId,
        decimal ValorTotal,
        string Status
    );

}
