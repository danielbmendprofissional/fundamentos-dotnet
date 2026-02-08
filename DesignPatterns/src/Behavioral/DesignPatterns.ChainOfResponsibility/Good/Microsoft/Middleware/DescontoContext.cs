using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.Microsoft.Middleware
{
    // OBRIGATORIEDADE: Representa o contexto que viaja pela corrente.
    public record DescontoContext(decimal Valor, bool Aprovado = false);
}
