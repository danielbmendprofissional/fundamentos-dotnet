using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Queries.ObterPedidoDetalhe
{
    public sealed record ObterPedidoDetalheQuery(Guid ClienteId, Guid Id)
        :IRequest<PedidoDetalheResponseDTO>;
}
