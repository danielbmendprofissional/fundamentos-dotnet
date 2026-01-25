using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.USA.Products
{
    public class CreditCard : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine("Processing payment with Credit Card...");
        }
    }
}
