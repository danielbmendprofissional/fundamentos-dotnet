using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Abstractions.Persistence;

namespace Vendas.Infrastructure.Persistence.Command
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly VendasDbContext _context;

        public UnitOfWork(VendasDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
