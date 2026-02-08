using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Bad
{
    // Classe com responsabilidade demais. 
    // Para cada novo nível de aprovação, você altera o mesmo método.
    public class ProcessadorDescontoBad
    {
        public void Aprovar(decimal valor)
        {
            if (valor < 100)
                Console.WriteLine("Vendedor aprovou.");
            else if (valor < 1000)
                Console.WriteLine("Gerente aprovou.");
            else if (valor < 5000)
                Console.WriteLine("Diretor aprovou.");
            else
                Console.WriteLine("Conselho aprovou.");
        }
    }
}
