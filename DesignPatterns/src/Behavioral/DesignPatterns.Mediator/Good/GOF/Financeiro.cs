using DesignPatterns.Mediator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF
{
    public class Financeiro : Component
    {
        public Financeiro(IMediator m) : base(m) { }

        public void Cobrar()
        {
            Console.WriteLine("[Financeiro] Pagamento processado.");
            _mediator.Notify(this, "PagamentoConcluido");
        }
    }
}
