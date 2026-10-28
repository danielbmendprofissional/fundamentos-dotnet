using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Good.GOF.Interfaces
{
    // OBRIGATORIEDADE: AbstractExpression (Interface base).
    public interface IExpressao
    {
        bool Interpretar(Contexto contexto);
    }
}
