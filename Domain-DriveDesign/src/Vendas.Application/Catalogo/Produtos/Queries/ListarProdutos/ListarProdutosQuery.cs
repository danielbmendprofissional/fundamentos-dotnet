using MediatR;

namespace Vendas.Application.Catalogo.Produtos.Queries.ListarProdutos
{
    public sealed record ListarProdutosQuery() : IRequest<IReadOnlyList<ProdutoResponseDTO>>;
}
