using DesignPatterns.Visitor.Good.Microsoft.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.Microsoft
{
    public static class CalculadorImposto
    {
        // OBRIGATORIEDADE: Tipagem forte. Aceitamos apenas IProduto.
        public static decimal Calcular(IProduto produto) => produto switch
        {
            Livro l => l.Preco * 0.05m,
            Eletronico e => e.Preco * 0.15m,
            _ => throw new ArgumentException("Produto não mapeado para imposto", nameof(produto))
        };
    }
}
