using System.ComponentModel;

namespace Vendas.Domain.Pedidos.Enums
{
    public enum StatusPedido
    {
        [Description("Pendente")]
        Pendente = 1,
        [Description("Pagamento Confirmado")]
        PagamentoConfirmado = 2,
        [Description("Em Separacao")]
        EmSeparacao = 3,
        [Description("Enviado")]
        Enviado = 4,
        [Description("Entregue")]
        Entregue = 5,
        [Description("Cancelado")]
        Cancelado = 6
    }
}
