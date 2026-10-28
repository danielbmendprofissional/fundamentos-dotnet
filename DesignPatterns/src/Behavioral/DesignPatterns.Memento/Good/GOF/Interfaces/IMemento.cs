using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Good.GOF.Interfaces
{
    // OBRIGATORIEDADE: Interface padrão GOF.
    public interface IMemento
    {
        string GetName();
        DateTime GetDate();
    }
}
