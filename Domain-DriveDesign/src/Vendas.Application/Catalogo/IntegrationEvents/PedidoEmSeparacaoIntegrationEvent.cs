using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Base;

namespace Vendas.Application.Catalogo.IntegrationEvents
{
    public sealed record PedidoEmSeparacaoIntegrationEvent(
        Guid PedidoId,
        IReadOnlyCollection<ProdutoReservaDTO> Produtos
    ) : IntegrationEvent;


}
