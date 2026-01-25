using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento.Brasil
{
    public class Credito : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine("Processando pagamento com cartão de crédito...");
        }
    }
}
