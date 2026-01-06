using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.AdicionarItemAoPedido
{
    public sealed record AdicionarItemAoPedidoCommand(
        Guid PedidoId,
        Guid ItemId,
        string NomeProduto,
        decimal PrecoUnitario,
        int Quantidade
    ) : IRequest<AdicionarItemAoPedidoResultDTO>;


}
