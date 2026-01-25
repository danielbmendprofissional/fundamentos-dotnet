using DesignPatterns.Factory.Solution.pure.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.enumWay
{
    public class PagamentoFactory
    {
        public static IPagamento Create(MetodoPagamento metodo)
        {
            switch (metodo)
            {
                case MetodoPagamento.Credito:
                    return new PagamentoCredito();
                case MetodoPagamento.Debito:
                    return new PagamentoDebito();
                case MetodoPagamento.Pix:
                    return new PagamentoPix();
                case MetodoPagamento.Boleto:
                    return new PagamentoBoleto();
                default:
                    throw new ArgumentException("Método de pagamento inválido.");
            }
        }
    }
}
