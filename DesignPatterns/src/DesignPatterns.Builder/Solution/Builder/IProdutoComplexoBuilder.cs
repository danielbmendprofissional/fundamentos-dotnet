using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Builder.Solution.Builder
{

    public interface IProdutoComplexoBuilder
    {
        IProdutoComplexoBuilder Reset();
        IProdutoComplexoBuilder DefinirIdentificacao(int id, string nome);
        IProdutoComplexoBuilder ConfigurarDimensoes(double peso);
        IProdutoComplexoBuilder AtribuirFornecedor(string nome, string cnpj);
        IProdutoComplexoBuilder IncluirItem(string nome, decimal preco);
    }
}
