using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Commands.RemoverItemPedido
{
    public sealed record RemoverItemPedidoCommand(
        Guid PedidoId,
        Guid ItemId
    ) : IRequest;

}
