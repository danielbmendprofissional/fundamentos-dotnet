using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Cache.Bad
{
    // Bad Situation when there is no proxy and the client accesses the real service directly,
    // causing performance issues when client needs to get the dollar price multiple times.
    public class CotacaoServiceReal : ICotacaoService
    {
        public decimal ObterPrecoDolar()
        {
            Console.WriteLine("[Real] Conectando ao Banco Central... (Lento)");
            Thread.Sleep(2500); // Network delay simulation
            return 5.20m + (decimal)new Random().NextDouble();
        }
    }
}
