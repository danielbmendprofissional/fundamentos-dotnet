using DesignPatterns.Builder.Solution.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Builder.Solution.Director
{
    internal class ProdutoComplexoDiretor : IProdutoComplexoDiretor
    {
        private readonly IProdutoComplexoBuilder _builder;

        public ProdutoComplexoDiretor(IProdutoComplexoBuilder builder)
        {
            _builder = builder;
        }

        public void ConstruirServidorDellPadrao()
        {
            _builder
                .Reset() 
                .DefinirIdentificacao(10, "Servidor PowerEdge")
                .ConfigurarDimensoes(12.0)
                .AtribuirFornecedor("Dell Brasil", "00.111.222/0001-33")
                .IncluirItem("Memória 16GB", 400m)
                .IncluirItem("HD 1TB", 300m);
        }

        public void ConstruirProdutoEmRascunho(string nome)
        {
            _builder
                .Reset()
                .DefinirIdentificacao(0, nome)
                .ConfigurarDimensoes(0);
        }
    }
}
