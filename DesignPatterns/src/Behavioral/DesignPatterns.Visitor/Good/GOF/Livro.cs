using DesignPatterns.Visitor.Good.GOF.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.GOF
{
    // ConcreteElementA
    public class Livro : IProduto
    {
        public string Titulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }

        // OBRIGATORIEDADE: Implementação de accept(v: Visitor).
        // Conforme o diagrama, ele chama: v.visitConcreteElementA(this);
        public void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
