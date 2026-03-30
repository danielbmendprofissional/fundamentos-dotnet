using System;
using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Identity.Entities;

namespace Onion.Domain.Identity.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(Account account, CancellationToken cancellationToken = default);
    }
}