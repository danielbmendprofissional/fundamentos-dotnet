using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Visitor.Good.GOF.Interface
{
    // ELEMENT
    // Define o contrato de aceitação para qualquer visitante.
    public interface IProduto
    {
        // OBRIGATORIEDADE: Método accept(v: Visitor) do diagrama.
        // É o "ponto de entrada" para algoritmos externos.
        void Accept(IVisitor visitor);
    }
}
