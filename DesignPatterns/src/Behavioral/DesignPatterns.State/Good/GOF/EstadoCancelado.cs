using DesignPatterns.State.Good.GOF.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.State.Good.GOF
{
    public class EstadoCancelado : PedidoState
    {
        public override void Pagar() => Console.WriteLine("[Erro] Impossível pagar pedido cancelado.");
        public override void Cancelar() => Console.WriteLine("[Erro] Pedido já está cancelado.");
    }
}
