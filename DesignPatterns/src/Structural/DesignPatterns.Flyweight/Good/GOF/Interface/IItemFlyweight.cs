using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Flyweight.Good.GOF.Interface
{
    // [Flyweight] no diagrama GoF.
    // Define o contrato para que o objeto pesado receba o "Estado Extrínseco".
    public interface IItemFlyweight
    {
        // Operation(extrinsicState)
        // O método recebe TUDO o que é único de cada instância para processar junto com o que ele já tem.
        void Display(Guid id, string owner, string name, string description);
    }
}
