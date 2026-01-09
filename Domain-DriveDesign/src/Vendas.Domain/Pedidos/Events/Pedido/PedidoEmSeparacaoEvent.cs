using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events.Pedido
{
    public sealed record PedidoItemSnapshot(Guid ProdutoId, int Quantidade);

    public sealed record PedidoEmSeparacaoEvent(
        Guid PedidoId,
        IReadOnlyCollection<PedidoItemSnapshot> Itens
    ) : DomainEvent;

}
