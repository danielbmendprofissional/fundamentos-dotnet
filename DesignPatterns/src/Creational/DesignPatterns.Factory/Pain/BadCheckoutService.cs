using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Pain
{
    public class BadCheckoutService
    {
        public void FinalizarCompra(decimal valor, string metodo)
        {
            // DOR: Acoplamento fortíssimo com classes concretas.
            // Se o Pix mudar a regra de autenticação, você mexe aqui.
            // Se entrar 'ApplePay', você mexe aqui.
            if (metodo == "Credito")
            {
                var credito = new PagamentoCredito();
                credito.Capturar(valor);
            }
            else if (metodo == "Debito")
            {
                var debito = new PagamentoDebito();
                debito.RealizarTransacao(valor);
            }
            else if (metodo == "Pix")
            {
                var pix = new PagamentoPix();
                pix.GerarQRCode(valor);
            }
            else if (metodo == "Boleto")
            {
                var boleto = new PagamentoBoleto();
                boleto.GerarLinhaDigitavel(valor);
            }
            // ... imagine isso para GooglePay, ValeRefeição, etc.
        }

        private class PagamentoCredito()
        {
            public void Capturar(decimal valor) =>
            Console.WriteLine("Processando pagamento com cartão de crédito...");
        }

        private class PagamentoDebito()
        {
            public void RealizarTransacao(decimal valor) =>
            Console.WriteLine("Processando pagamento com cartão de débito...");
        }

        private class PagamentoPix()
        {
            public void GerarQRCode(decimal valor) =>
            Console.WriteLine("Processando pagamento via Pix...");
        }

        private class PagamentoBoleto()
        {
            public void GerarLinhaDigitavel(decimal valor) =>
            Console.WriteLine("Processando pagamento via Boleto...");
        }
    }

}
