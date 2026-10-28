using DesignPatterns.Mediator.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.GOF
{
    // OBRIGATORIEDADE: Classe base para os componentes (Colleagues).
    // Garante que cada peça do sistema tenha acesso à "Torre de Controle".
    public abstract class Component
    {
        protected IMediator _mediator;
        public Component(IMediator mediator) => _mediator = mediator;
    }
}
