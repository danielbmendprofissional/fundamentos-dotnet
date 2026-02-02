using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Iterator.Good.GOF.Abstractions
{
    // OBRIGATORIEDADE: Interface para a coleção (Agregado). 
    // Ela garante que qualquer coleção saiba entregar seu próprio navegador.
    public interface ICollection
    {
        IIterator CreateIterator();
    }
}
