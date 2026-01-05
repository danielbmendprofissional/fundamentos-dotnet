using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Enum;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Pedidos.Events.Pedido
{
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    //Sealed sem herança
    public sealed record PedidoCanceladoEvent(
        Guid PedidoId,
        Guid ClienteId,
        StatusPedido StatusAnterior,
        MotivoCancelamento Motivo,
        Guid? PagamentoId
    ) : DomainEvent;

}
