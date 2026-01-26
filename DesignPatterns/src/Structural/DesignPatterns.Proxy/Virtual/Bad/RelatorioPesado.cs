using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Virtual.Bad
{
    //Bad way, complex object is created always at the beginning
    internal class RelatorioPesado : IRelatorio
    {
        public RelatorioPesado()
        {
            Console.WriteLine("[Real] Carregando 500MB de dados... (Lento)");
        }
        public void Exibir() => Console.WriteLine("[Real] Relatório renderizado.");
    }
}
