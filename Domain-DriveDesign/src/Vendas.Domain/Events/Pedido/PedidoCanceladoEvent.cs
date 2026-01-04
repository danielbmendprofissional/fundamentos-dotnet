using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Enum;
using Vendas.Domain.ValueObjects;

namespace Vendas.Domain.Events.Pedido
{   
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    //Sealed sem herança e sem alteração
    public sealed record PedidoCanceladoEvent(
        Guid PedidoId,
        Guid ClienteId,
        StatusPedido StatusAnterior,
        MotivoCancelamento Motivo,
        Guid? PagamentoId
    ) : DomainEvent;

}
