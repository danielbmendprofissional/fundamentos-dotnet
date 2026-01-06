using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Queries.Pedidos.Commom;

namespace Vendas.Application.Queries.Pedidos.ListarPedidosCliente
{
    public sealed record ListarPedidosDoClienteQuery(Guid ClienteId)
        : IRequest<IReadOnlyList<PedidoResponseDTO>>;

}
