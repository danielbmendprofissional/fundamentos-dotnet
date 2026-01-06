using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Pedidos.Snapshot
{
    public sealed record PedidoItemSnapshot(
        Guid ProdutoId,
        int Quantidade
    );

}
