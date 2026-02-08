using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.GOF.Handler
{
    // OBRIGATORIEDADE: Classe abstrata.
    // Note o campo 'sucessor' que permite a recursividade da corrente.
    public abstract class AprovaDescontoHandler
    {
        protected AprovaDescontoHandler _sucessor;

        // Método para montar a corrente
        public void SetSucessor(AprovaDescontoHandler sucessor)
        {
            _sucessor = sucessor;
        }

        // OBRIGATORIEDADE: O método handleRequest().
        public abstract void ProcessarSolicitacao(decimal valor);
    }
}
