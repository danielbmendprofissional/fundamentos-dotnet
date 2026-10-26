using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Interfaces;
using Vendas.Application.Catalogo.Categorias.Queries.ListarCategorias;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;

namespace Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes
{
    public sealed class ObterCategoriaDetalhesQueryHandler
        : IRequestHandler<ObterCategoriaDetalhesQuery, CategoriaDetalhesResponseDTO>
    {
        private readonly ICategoriaQueryRepository _repository;

        public ObterCategoriaDetalhesQueryHandler(ICategoriaQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoriaDetalhesResponseDTO> Handle(
            ObterCategoriaDetalhesQuery request,
            CancellationToken cancellationToken)
        {
            var retorno = await _repository.ObterDetalheAsync(request.Id, cancellationToken);

            if (retorno == null)
                throw new ArgumentNullException("Categoria não encontrada.");

            return retorno;
        }
    }
}
