using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Bad
{
    public class EstoqueBad
    {
        public bool Validar(string produto)
        {
            Console.WriteLine($"[Estoque] Validando item: {produto}");
            return true;
        }
    }
}
