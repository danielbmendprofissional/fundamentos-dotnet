using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento.USA
{
    public class Paypal : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine("Processing payment with Paypal...");
        }
    }
}
