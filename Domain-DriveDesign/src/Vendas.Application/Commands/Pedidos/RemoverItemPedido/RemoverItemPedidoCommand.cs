using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.RemoverItemPedido
{
    public sealed record RemoverItemPedidoCommand(
        Guid PedidoId,
        Guid ItemId
    ) : IRequest;

}
