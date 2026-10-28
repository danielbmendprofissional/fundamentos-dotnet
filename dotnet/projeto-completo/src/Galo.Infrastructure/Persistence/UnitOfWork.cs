using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly GaloDbContext _context;

        public UnitOfWork(GaloDbContext context) => _context = context;

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // A Application não conhece EF: traduzimos para uma exceção própria.
                throw new ConcurrencyConflictException("Os dados foram alterados por outro processo. Tente novamente.", ex);
            }
        }

        // Após um conflito, o que está no ChangeTracker está "velho": limpar para reler do banco.
        public void DiscardChanges() => _context.ChangeTracker.Clear();
    }
}
