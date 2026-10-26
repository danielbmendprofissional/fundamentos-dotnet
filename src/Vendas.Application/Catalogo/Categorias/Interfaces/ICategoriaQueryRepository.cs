using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;

namespace Vendas.Application.Catalogo.Categorias.Interfaces
{
    public interface ICategoriaQueryRepository
    {
        Task<IReadOnlyList<CategoriaResponseDTO>> ListarAsync(
            CancellationToken cancellationToken);

        Task<CategoriaDetalhesResponseDTO?> ObterDetalheAsync(
            Guid id,
            CancellationToken cancellationToken);
    }
}
