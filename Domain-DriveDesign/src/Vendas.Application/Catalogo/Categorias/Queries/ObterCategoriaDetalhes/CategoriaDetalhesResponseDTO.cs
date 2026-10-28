using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes
{
    public sealed record CategoriaDetalhesResponseDTO(
            Guid Id,
            string Nome,
            string Descricao,
            bool Ativo
        );
}
