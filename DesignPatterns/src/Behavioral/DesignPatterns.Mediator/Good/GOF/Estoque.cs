using DesignPatterns.Mediator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF
{
    public class Estoque : Component
    {
        public Estoque(IMediator m) : base(m) { }

        public void Validar()
        {
            Console.WriteLine("[Estoque] Itens reservados com sucesso.");
            _mediator.Notify(this, "EstoqueValidado");
        }
    }
}
