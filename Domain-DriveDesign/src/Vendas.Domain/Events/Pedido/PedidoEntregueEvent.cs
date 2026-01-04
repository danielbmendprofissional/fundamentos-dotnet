using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Events.Pedido
{
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    //Sealed sem herança e sem alteração
    public sealed record PedidoEntregueEvent(
        Guid PedidoId,
        Guid ClienteId
    ) : DomainEvent;

}
