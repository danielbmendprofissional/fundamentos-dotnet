using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;

namespace Vendas.Application.Catalogo.Categorias.Queries.ListarCategorias
{
    public sealed record ListarCategoriasQuery() : IRequest<IReadOnlyList<CategoriaResponseDTO>>;
}
