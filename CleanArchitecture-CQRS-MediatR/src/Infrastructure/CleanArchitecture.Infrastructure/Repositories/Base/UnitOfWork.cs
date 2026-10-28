using CleanArchitecture.Domain.Interfaces.Repository.Base;
using CleanArchitecture.Infrastructure.Persistence.Context;

namespace CleanArchitecture.Infrastructure.Repositories.Base
{
    /*
        Implementation of IUnitOfWork
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
