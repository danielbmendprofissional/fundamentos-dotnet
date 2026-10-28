using DesignPatterns.Visitor.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.GOF
{
    // ConcreteVisitor2: Algoritmo de Exportação
    public class PDFVisitor : IVisitor
    {
        public void Visit(Livro livro)
            => Console.WriteLine($"[Visitor PDF] Gerando página para o livro: {livro.Titulo}");

        public void Visit(Eletronico eletronico)
            => Console.WriteLine($"[Visitor PDF] Gerando ficha técnica para: {eletronico.Nome}");
    }
}
