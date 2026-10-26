using System.ComponentModel;

namespace Vendas.Domain.Pedidos.Enums
{
    public enum StatusPagamento
    {
        [Description("Pendente")]
        Pendente = 1,
        [Description("Aprovado")]
        Aprovado = 2,
        [Description("Recusado")]
        Recusado = 3,
        [Description("Estornado")]
        Estornado = 4,
        [Description("Cancelado")]
        Cancelado = 5
    }
}
