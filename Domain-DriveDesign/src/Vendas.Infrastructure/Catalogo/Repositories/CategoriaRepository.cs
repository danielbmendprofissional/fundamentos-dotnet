using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Interfaces.Categorias;
using Vendas.Infrastructure.Common.Persistence;

namespace Vendas.Infrastructure.Catalogo.Repositories
{
    internal sealed class CategoriaRepository : ICategoriaRepository
    {
        private readonly VendasDbContext _context;

        public CategoriaRepository(VendasDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task AdicionarAsync(
            Categoria categoria,
            CancellationToken cancellationToken = default)
        {
            await _context.Categorias.AddAsync(categoria, cancellationToken);
        }

        public void Atualizar(Categoria categoria)
        {
            var entry = _context.Entry(categoria);
            if (entry.State == EntityState.Detached)
            {
                _context.Categorias.Update(categoria);
            }
        }
    }
}
