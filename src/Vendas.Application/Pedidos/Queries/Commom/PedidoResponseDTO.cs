using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Queries.Commom
{
    public sealed record PedidoResponseDTO(
        Guid Id,
        string NumeroPedido,
        decimal ValorTotal,
        string Status,
        DateTime DataCriacao);

}
