namespace Vendas.Application.Catalogo.Produtos.Queries.ListarProdutos
{
    public sealed record ProdutoResponseDTO(
            Guid Id,
            string Nome,
            decimal Preco,
            string? Descricao
        );
}
