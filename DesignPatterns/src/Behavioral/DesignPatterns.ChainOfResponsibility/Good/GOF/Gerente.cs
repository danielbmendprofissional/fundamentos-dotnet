using DesignPatterns.ChainOfResponsibility.Good.GOF.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.GOF
{
    // OBRIGATORIEDADE: Elos concretos da corrente.
    public class Gerente : AprovaDescontoHandler
    {
        public override void ProcessarSolicitacao(decimal valor)
        {
            if (valor < 1000)
                Console.WriteLine($"[Gerente] Eu aprovo o desconto de R${valor}.");
            else if (_sucessor != null)
                _sucessor.ProcessarSolicitacao(valor);
        }
    }
}
