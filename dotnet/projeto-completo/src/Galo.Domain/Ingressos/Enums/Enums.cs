namespace Galo.Domain.Ingressos.Enums
{
    /// <summary>
    /// Ciclo de vida da compra. A ORDEM importa: "status >= IngressosEmitidos" significa
    /// "já não dá mais para cancelar".
    /// </summary>
    public enum StatusCompra
    {
        Pendente = 1,              // montando o carrinho
        AguardandoPagamento = 2,   // pagamento iniciado, reserva de ingressos solicitada
        PagamentoConfirmado = 3,
        IngressosEmitidos = 4,
        Cancelada = 5
    }

    public enum MetodoPagamento { Pix = 1, CartaoCredito = 2, CartaoDebito = 3, Boleto = 4 }

    public enum StatusPagamento { Pendente = 1, Aprovado = 2, Recusado = 3 }
}
