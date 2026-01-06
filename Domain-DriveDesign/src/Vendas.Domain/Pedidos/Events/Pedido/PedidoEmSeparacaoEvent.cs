using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Pedidos.Snapshot;

namespace Vendas.Domain.Pedidos.Events.Pedido
{
    public sealed record PedidoEmSeparacaoEvent(
        Guid PedidoId,
        IReadOnlyCollection<PedidoItemSnapshot> Itens
    ) : DomainEvent;

}
