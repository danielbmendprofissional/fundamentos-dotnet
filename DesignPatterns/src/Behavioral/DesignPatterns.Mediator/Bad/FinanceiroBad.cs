using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Bad
{
    public class FinanceiroBad
    {
        public void Cobrar(decimal valor) => Console.WriteLine($"[Financeiro] Cobrando R$ {valor}");
    }
}
