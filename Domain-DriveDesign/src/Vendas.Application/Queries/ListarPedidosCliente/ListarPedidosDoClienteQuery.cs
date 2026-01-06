using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Queries.Commom;

namespace Vendas.Application.Queries.ListarPedidos
{
    public sealed record ListarPedidosDoClienteQuery(Guid ClienteId)
        : IRequest<IReadOnlyList<PedidoResponseDTO>>;

}
