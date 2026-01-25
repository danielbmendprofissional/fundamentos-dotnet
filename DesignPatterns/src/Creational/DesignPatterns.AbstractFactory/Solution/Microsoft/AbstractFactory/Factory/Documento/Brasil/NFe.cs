using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento.Brasil
{
    public class NFe : IDocumento
    {
        public void Gerar(decimal valor) =>
            Console.WriteLine($"Emitindo Nota Fiscal Eletrônica (NF-e) no valor de R${valor}");
    }
}
