using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events.Pagamento
{
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    //Sealed sem herança
    public sealed record PagamentoAprovadoEvent(Guid PagamentoId,
                                         Guid PedidoId,
                                         decimal Valor,
                                         DateTime DataPagamento,
                                         string? CodigoTransacao) : DomainEvent;
}
