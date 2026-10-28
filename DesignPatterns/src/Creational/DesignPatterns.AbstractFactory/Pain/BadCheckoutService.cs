using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Pain
{
    public class BadCheckoutService
    {
        public void ProcessarPagamento(decimal valor, string pais, string metodo)
        {
            // --- CENÁRIO BRASIL ---
            if (pais == "Brasil")
            {
                // The Developer has to remember to switch to the Brazilian tax rule
                Console.WriteLine("Aplicando impostos brasileiros (ICMS/ISS)...");

                if (metodo == "Pix")
                {
                    var pix = new PixService();
                    pix.GerarQRCode(valor);
                }
                else if (metodo == "Credito")
                {
                    var cartao = new CartaoBrasilService();
                    cartao.Capturar(valor);
                }
                else if (metodo == "Debito")
                {
                    var debito = new DebitoBrasilService();
                    debito.Descontar(valor);
                }
                else if (metodo == "Boleto")
                {
                    var boleto = new BoletoService();
                    boleto.Emitir(valor);
                }
            }

            // --- CENÁRIO USA ---
            else if (pais == "USA")
            {
                // The Developer has to remember to switch to the USA tax rule
                Console.WriteLine("Applying USA Sales Tax...");

                if (metodo == "Paypal")
                {
                    var paypal = new PaypalService();
                    paypal.Charge(valor);
                }
                else if (metodo == "Credito")
                {
                    var cartao = new CartaoUSAService();
                    cartao.Authorize(valor);
                }
                else if (metodo == "Debito")
                {
                    var debito = new DebitoUSAService();
                    debito.Withdraw(valor);
                }
                else if (metodo == "Seila")
                {
                    var seila = new OutroMetodoUSAService();
                    seila.Process(valor);
                }
            }
        }

        private class PixService
        {
            public void GerarQRCode(decimal valor)
            {
                Console.WriteLine($"Gerando QR Code para pagamento via Pix no valor de {valor:C}.");
            }
        }
        private class CartaoBrasilService
        {
            public void Capturar(decimal valor)
            {
                Console.WriteLine($"Capturando pagamento com cartão de crédito no Brasil no valor de {valor:C}.");
            }
        }
        private class DebitoBrasilService
        {
            public void Descontar(decimal valor)
            {
                Console.WriteLine($"Descontando pagamento com cartão de débito no Brasil no valor de {valor:C}.");
            }
        }
        private class BoletoService
        {
            public void Emitir(decimal valor)
            {
                Console.WriteLine($"Emitindo boleto bancário no valor de {valor:C}.");
            }
        }
        private class PaypalService
        {
            public void Charge(decimal valor)
            {
                Console.WriteLine($"Charging payment via PayPal in the amount of {valor:C}.");
            }
        }
        private class CartaoUSAService
        {
            public void Authorize(decimal valor)
            {
                Console.WriteLine($"Authorizing credit card payment in the USA in the amount of {valor:C}.");
            }
        }
        private class DebitoUSAService
        {
            public void Withdraw(decimal valor)
            {
                Console.WriteLine($"Withdrawing debit card payment in the USA in the amount of {valor:C}.");
            }
        }
        private class OutroMetodoUSAService
        {
            public void Process(decimal valor)
            {
                Console.WriteLine($"Processing payment with another USA method in the amount of {valor:C}.");
            }
        }
    }
}