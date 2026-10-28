using DesignPatterns.Visitor.Bad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.GOF.Interface
{
    // VISITOR
    // Corresponde à interface que define as operações de visita para cada tipo concreto.
    public interface IVisitor
    {
        // Corresponde a visitConcreteElementA
        void Visit(Livro livro);

        // Corresponde a visitConcreteElementB
        void Visit(Eletronico eletronico);
    }
}
