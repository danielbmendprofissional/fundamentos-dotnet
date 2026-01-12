using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Catalogo.Produtos.Repositories
{
    internal sealed class ProdutoRepository : IProdutoRepository
    {
        private readonly VendasDbContext _context;

        public ProdutoRepository(VendasDbContext context)
        {
            _context = context;
        }

        public async Task<Produto?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Produto>> ObterPorIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
        {
            return await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                // IMPORTANTE: Como é repositório de escrita para ajuste de estoque,
                // NÃO use AsNoTracking(), pois o ChangeTracker precisa monitorar
                // as entidades para o UnitOfWork persistir as mudanças.
                .ToListAsync(cancellationToken);
        }
        public async Task AdicionarAsync(
            Produto produto,
            CancellationToken cancellationToken = default)
        {
            await _context.Produtos.AddAsync(produto, cancellationToken);
        }

        public void Atualizar(Produto produto)
        {
            var entry = _context.Entry(produto);
            if (entry.State == EntityState.Detached)
            {
                _context.Produtos.Update(produto);
            }
        }
    }
}
