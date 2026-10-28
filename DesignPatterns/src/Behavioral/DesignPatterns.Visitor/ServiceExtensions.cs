using DesignPatterns.Visitor.Good.Microsoft;
using DesignPatterns.Visitor.Good.Microsoft.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor
{
    public static class ServiceExtensions { }

    public static class ProdutoExtensions
    {
        // OBRIGATORIEDADE: Extension Method simulando o comportamento do objeto.
        public static decimal GetTax(this IProduto produto) => CalculadorImposto.Calcular(produto);
    }
}
