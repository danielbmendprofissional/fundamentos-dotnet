using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels
{
    public sealed class ImagemProdutoModel
    {
        public Guid ProdutoId { get; init; }
        public string Url { get; init; } = string.Empty;
        public int Ordem { get; init; }
    }
}
