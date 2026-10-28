using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator._2.Base
{
    public class PedidoBase : IPedido
    {
        public decimal CalcularTotal() => 100.00m; // Just for example
        public string GetDescricao() => "Pedido Comum ($100)";
    }
}
