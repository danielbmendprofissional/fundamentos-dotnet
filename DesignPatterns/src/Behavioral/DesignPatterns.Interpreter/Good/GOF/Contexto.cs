using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Interpreter.Good.GOF
{
    // OBRIGATORIEDADE: Contexto que carrega informações globais (ex: valores das variáveis).
    public class Contexto
    {
        public int ValorReferencia { get; set; }
    }
}
