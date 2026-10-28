using DesignPatterns.ChainOfResponsibility.Good.GOF.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ChainOfResponsibility.Good.GOF
{
    // OBRIGATORIEDADE: Elos concretos da corrente.
    // O último do fluxo
    public class Diretor : AprovaDescontoHandler
    {
        public override void ProcessarSolicitacao(decimal valor)
        {
            Console.WriteLine($"[Diretor] Como última instância, aprovo R${valor}.");
        }
    }
}
