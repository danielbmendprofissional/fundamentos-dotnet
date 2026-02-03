using DesignPatterns.Mediator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF
{
    public class Carrinho : Component
    {
        public Carrinho(IMediator m) : base(m) { }

        public void Finalizar()
        {
            Console.WriteLine("[Carrinho] Cliente clicou em finalizar.");
            _mediator.Notify(this, "CheckoutIniciado");
        }
    }
}
