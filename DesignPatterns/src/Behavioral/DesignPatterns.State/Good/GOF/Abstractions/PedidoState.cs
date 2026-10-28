using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Good.GOF.Abstractions
{
    // OBRIGATORIEDADE: Abstração state.
    public abstract class PedidoState
    {
        protected PedidoContext _context;
        public void SetContext(PedidoContext context) => _context = context;

        public abstract void Pagar();
        public abstract void Cancelar();
    }
}
