using DesignPatterns.State.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Good.GOF
{
    // Estados Concretos
    public class EstadoPago : PedidoState
    {
        public override void Pagar() => Console.WriteLine("[Erro] Pedido já está pago.");

        public override void Cancelar()
        {
            Console.WriteLine("[Pago] Estornando valor e cancelando pedido.");
            _context.TransitionTo(new EstadoCancelado());
        }
    }
}
