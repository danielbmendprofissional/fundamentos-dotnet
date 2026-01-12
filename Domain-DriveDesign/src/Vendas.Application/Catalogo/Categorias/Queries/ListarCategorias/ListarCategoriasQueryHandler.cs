using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Interfaces;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;

namespace Vendas.Application.Catalogo.Categorias.Queries.ListarCategorias
{
    public sealed class ListarCategoriasQueryHandler
        : IRequestHandler<ListarCategoriasQuery, IReadOnlyList<CategoriaResponseDTO>>
    {
        private readonly ICategoriaQueryRepository _repository;

        public ListarCategoriasQueryHandler(ICategoriaQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CategoriaResponseDTO>> Handle(
            ListarCategoriasQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.ListarAsync(cancellationToken);
        }
    }
}
