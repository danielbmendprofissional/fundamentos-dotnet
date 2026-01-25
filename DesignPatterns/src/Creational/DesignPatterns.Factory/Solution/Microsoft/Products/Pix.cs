using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.Microsoft.Products
{
    public class Pix : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine($"Processando pagamento de R$ {valor} via Pix.");
        }
    }
}
