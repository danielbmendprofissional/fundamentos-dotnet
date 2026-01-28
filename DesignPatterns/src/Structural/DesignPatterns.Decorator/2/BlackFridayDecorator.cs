using DesignPatterns.Decorator._2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Decorator._2
{

    //So, here we have a simple Decorator that applies a Black Friday discount to an existing order.
    //The base remains the same, but now to the client, the order has a new behavior (a discount).

    public class BlackFridayDecorator : IPedido
    {
        private readonly IPedido _pedido;

        public BlackFridayDecorator(IPedido pedido) => _pedido = pedido;

        public decimal CalcularTotal() => _pedido.CalcularTotal() * 0.80m; // -20%

        public string GetDescricao() => _pedido.GetDescricao() + " + Desconto Black Friday";
    }
}
