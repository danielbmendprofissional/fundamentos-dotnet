using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Builder.Solution.Director
{
    public interface IProdutoComplexoDiretor
    {
        void ConstruirServidorDellPadrao();
        void ConstruirProdutoEmRascunho(string nome);
    }
}
