using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Enums.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels
{
    public sealed class ProdutoResumoModel
    {
        public Guid Id { get; init; }
        public string Nome { get; init; } = string.Empty; // NomeProduto -> string
        public decimal Preco { get; init; }              // PrecoProduto -> decimal
        public int Status { get; init; }                 // StatusProduto -> int
        public string? Descricao { get; init; }
    }
}
