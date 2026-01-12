using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes
{
    public sealed record ObterCategoriaDetalhesQuery(
            Guid Id
        ) : IRequest<CategoriaDetalhesResponseDTO>;
}
