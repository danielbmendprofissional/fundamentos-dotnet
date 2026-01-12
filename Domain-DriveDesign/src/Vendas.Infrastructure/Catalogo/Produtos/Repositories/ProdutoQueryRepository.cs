using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Categorias.Interfaces;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaDetalhes;
using Vendas.Application.Catalogo.Categorias.Queries.ObterCategoriaResumo;
using Vendas.Application.Catalogo.Produtos.Interfaces;
using Vendas.Application.Catalogo.Produtos.Queries.ListarProdutos;
using Vendas.Application.Catalogo.Produtos.Queries.ObterProdutoDetalhes;
using Vendas.Application.Commom.Extensions;
using Vendas.Domain.Catalogo.Enums.Produtos;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Catalogo.Produtos.Repositories
{
    public sealed class ProdutoQueryRepository
        : IProdutoQueryRepository
    {
        private readonly VendasQueryDbContext _context;

        public ProdutoQueryRepository(
            VendasQueryDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ProdutoResponseDTO>> ListarAsync(
                CancellationToken cancellationToken)
        {
            return await _context.ProdutoResumo
                .AsNoTracking()
                .Where(x => x.Status == 1)
                .Select(p => new ProdutoResponseDTO(
                    p.Id,
                    p.Nome,
                    p.Preco,
                    p.Descricao
                    )
                 ).ToListAsync(cancellationToken);
        }

        public async Task<ProdutoDetalhesResponseDTO?> ObterDetalheAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            // Busca os dados brutos (Model de leitura)
            var produto = await _context.ProdutoDetalhes
                .AsNoTracking()
                .Include(p => p.Imagens) // Se não estiver usando Select direto, precisa do Include
                .Where(p => p.Id == id && p.Status == 1)
                .SingleOrDefaultAsync(cancellationToken);

            if (produto == null) return null;

            // Mapeia para o DTO usando a Extension de Application
            return new ProdutoDetalhesResponseDTO(
                produto.Id,
                produto.Nome,
                produto.Codigo,
                produto.Preco,
                produto.Descricao,
                produto.CategoriaId,
                ((StatusProduto)produto.Status).GetDescription(), // Aqui entra a Extensions
                produto.Estoque,
                produto.Imagens.Select(i => new ImagemProdutoResponseDTO(i.Url, i.Ordem)).ToList()
            );
        }
    }
}
