using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.Queries.ObterProdutoDetalhes
{
    public sealed record ProdutoDetalhesResponseDTO(
            Guid Id,
            string Nome,
            string Codigo,
            decimal Preco,
            string? Descricao,
            Guid CategoriaId,
            string Status,
            int Estoque,
            IReadOnlyList<ImagemProdutoResponseDTO> Imagens
        );
}
