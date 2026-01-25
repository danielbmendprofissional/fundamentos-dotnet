using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.pure.Products
{
    //product
    public class PagamentoPix : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine("Processando pagamento via Pix...");
        }
    }
}
