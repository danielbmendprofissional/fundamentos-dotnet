using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF.Abstractions
{
    // OBRIGATORIEDADE
    // O Mediador é o centro de inteligência que recebe notificações.
    public interface IMediator
    {
        //object é gambiarrinha, mas é somente um exemplo purista do GOF
        void Notify(object sender, string evento);
    }
}
