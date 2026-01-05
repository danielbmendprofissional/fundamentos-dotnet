using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events.Pedido
{
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    //Sealed sem herança
    public sealed record PedidoEntregueEvent(
        Guid PedidoId,
        Guid ClienteId
    ) : DomainEvent;

}
