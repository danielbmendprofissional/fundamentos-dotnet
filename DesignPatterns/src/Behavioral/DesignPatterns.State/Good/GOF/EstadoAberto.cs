using DesignPatterns.State.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Good.GOF
{
    // Estados Concretos
    public class EstadoAberto : PedidoState
    {
        public override void Pagar()
        {
            Console.WriteLine("[Aberto] Pagamento realizado com sucesso.");
            _context.TransitionTo(new EstadoPago()); // Transição lógica
        }

        public override void Cancelar() => Console.WriteLine("[Aberto] Pedido cancelado.");
    }
}
