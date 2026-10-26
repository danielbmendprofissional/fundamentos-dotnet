using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;
using Vendas.Application.Catalogo.Produtos.Queries.ListarProdutos;
using Vendas.Application.Catalogo.Produtos.Queries.ObterProdutoDetalhes;

namespace Vendas.Application.Catalogo.Produtos.Interfaces
{
    public interface IProdutoQueryRepository
    {
        Task<IReadOnlyList<ProdutoResponseDTO>> ListarAsync(
            CancellationToken cancellationToken);

        Task<ProdutoDetalhesResponseDTO?> ObterDetalheAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
