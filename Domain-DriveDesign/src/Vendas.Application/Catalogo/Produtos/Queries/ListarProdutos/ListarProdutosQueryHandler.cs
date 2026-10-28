using MediatR;
using Vendas.Application.Catalogo.Produtos.Interfaces;

namespace Vendas.Application.Catalogo.Produtos.Queries.ListarProdutos
{
    public sealed class ListarProdutosQueryHandler
        : IRequestHandler<ListarProdutosQuery, IReadOnlyList<ProdutoResponseDTO>>
    {
        private readonly IProdutoQueryRepository _repository;

        public ListarProdutosQueryHandler(IProdutoQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<ProdutoResponseDTO>> Handle(
            ListarProdutosQuery request,
            CancellationToken cancellationToken)
        {
            return await _repository.ListarAsync(cancellationToken);
        }
    }
}
