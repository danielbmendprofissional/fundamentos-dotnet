using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Enums.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels
{
    public sealed class ProdutoDetalhesModel
    {
        public Guid Id { get; }
        public string Nome { get; init; } = string.Empty; // NomeProduto -> string
        public string Codigo { get; private set; } = string.Empty; //CodigoProduto -> string
        public decimal Preco { get; init; }              // PrecoProduto -> decimal
        public string Descricao { get; } = string.Empty;
        public Guid CategoriaId { get; private set; }
        public int Status { get; init; }                 // StatusProduto -> int
        public int Estoque { get; private set; }
        public IReadOnlyList<ImagemProdutoModel> Imagens { get; } = new List<ImagemProdutoModel>();
    }
}
