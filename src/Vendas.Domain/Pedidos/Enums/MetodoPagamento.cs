using System.ComponentModel;

namespace Vendas.Domain.Pedidos.Enums
{
    public enum MetodoPagamento
    {
        [Description("Cartão de crédito")]
        CartaoCredito = 1,
        [Description("Cartão de débito")]
        CartaoDebito = 2,
        [Description("Pix")]
        Pix = 3,
        [Description("Boleto")]
        Boleto = 4,
        [Description("Transferência Bancaria")]
        TransferenciaBancaria = 5
    }
}
