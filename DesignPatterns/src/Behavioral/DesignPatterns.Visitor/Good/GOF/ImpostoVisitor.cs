using DesignPatterns.Visitor.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.GOF
{
    // ConcreteVisitor1: Algoritmo de Impostos
    public class ImpostoVisitor : IVisitor
    {
        public void Visit(Livro livro)
            => Console.WriteLine($"[Visitor Imposto] Livro {livro.Titulo}: R${livro.Preco * 0.05m}");

        public void Visit(Eletronico eletronico)
            => Console.WriteLine($"[Visitor Imposto] Eletrônico {eletronico.Nome}: R${eletronico.Preco * 0.15m}");
    }
}
