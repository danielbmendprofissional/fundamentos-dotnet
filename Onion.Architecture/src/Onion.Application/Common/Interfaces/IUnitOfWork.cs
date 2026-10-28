using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
        Task BeginTransactionAsync();
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync();
    }
}
