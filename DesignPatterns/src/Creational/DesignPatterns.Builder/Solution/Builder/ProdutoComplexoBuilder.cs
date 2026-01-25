using DesignPatterns.Builder.Solution.Classes;

namespace DesignPatterns.Builder.Solution.Builder
{
    internal class ProdutoComplexoBuilder : IProdutoComplexoBuilder
    {
        private ProdutoComplexoAnemico _produto;

        public ProdutoComplexoBuilder()
        {
            _produto = new ProdutoComplexoAnemico();
        }

        public IProdutoComplexoBuilder Reset()
        {
            _produto = new ProdutoComplexoAnemico();
            return this;
        }

        public IProdutoComplexoBuilder DefinirIdentificacao(int id, string nome)
        {
            _produto.Id = id;
            _produto.Nome = nome;
            return this;
        }

        public IProdutoComplexoBuilder ConfigurarDimensoes(double peso)
        {
            _produto.Peso = peso;
            return this;
        }

        public IProdutoComplexoBuilder AtribuirFornecedor(string nome, string cnpj)
        {
            _produto.Fornecedor = new FornecedorAnemico { NomeFantasia = nome, CNPJ = cnpj };
            return this;
        }

        public IProdutoComplexoBuilder IncluirItem(string nome, decimal preco)
        {
            _produto.ItensRelacionados.Add(new ItemPedidoAnemico { Nome = nome, Preco = preco });
            return this;
        }

        public ProdutoComplexoAnemico Build()
        {
            var resultado = _produto;
            Reset();
            return resultado;
        }
    }
}
