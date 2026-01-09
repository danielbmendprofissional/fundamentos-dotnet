using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Pedidos.Queries.Commom;

namespace Vendas.Application.Pedidos.Queries.ListarPedidosCliente
{
    public sealed record ListarPedidosDoClienteQuery(Guid ClienteId)
        : IRequest<IReadOnlyList<PedidoResponseDTO>>;

}
