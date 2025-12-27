using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.OCP.Good.@base
{
    public abstract class Discount
    {
        // -- marcar o método como abstract informa ao código que outra classe vai implementar esse método.
        public abstract decimal Calculate(decimal valor);
    }
}
