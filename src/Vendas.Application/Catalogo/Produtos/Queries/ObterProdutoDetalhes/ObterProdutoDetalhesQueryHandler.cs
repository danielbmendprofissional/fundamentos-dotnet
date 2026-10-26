using MediatR;
using Vendas.Application.Catalogo.Produtos.Interfaces;

namespace Vendas.Application.Catalogo.Produtos.Queries.ObterProdutoDetalhes
{
    public sealed class ObterProdutoDetalhesQueryHandler
        : IRequestHandler<ObterProdutoDetalhesQuery, ProdutoDetalhesResponseDTO>
    {
        private readonly IProdutoQueryRepository _repository;

        public ObterProdutoDetalhesQueryHandler(IProdutoQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProdutoDetalhesResponseDTO> Handle(
            ObterProdutoDetalhesQuery request,
            CancellationToken cancellationToken)
        {
            var retorno = await _repository.ObterDetalheAsync(request.Id, cancellationToken);

            if (retorno == null)
                throw new ArgumentNullException("Produto não encontrado.");

            return retorno;
        }
    }
}
