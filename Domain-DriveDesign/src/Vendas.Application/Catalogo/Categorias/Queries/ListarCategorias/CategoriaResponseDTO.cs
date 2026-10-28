using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo
{
    public sealed record CategoriaResponseDTO(
            Guid Id,
            string Nome
        );
}
