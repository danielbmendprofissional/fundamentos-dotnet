using CleanArchitecture.Domain.Interfaces.Repository.Base;
using CleanArchitecture.Persistence.Context;

namespace CleanArchitecture.Persistence.Repositories.Base
{
    /*
        Implementação do IUnitOfWork
    */
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
