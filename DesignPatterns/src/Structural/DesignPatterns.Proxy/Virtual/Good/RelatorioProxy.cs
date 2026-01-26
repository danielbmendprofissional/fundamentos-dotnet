using DesignPatterns.Proxy.Virtual.Bad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Virtual.Good
{
    public class RelatorioProxy : IRelatorio
    {
        private RelatorioPesado? _objetoReal;

        public void Exibir()
        {
            // If the real object is not created yet, create it, otherwise use the existing one
            _objetoReal ??= new RelatorioPesado();
            _objetoReal.Exibir();
        }
    }
}
