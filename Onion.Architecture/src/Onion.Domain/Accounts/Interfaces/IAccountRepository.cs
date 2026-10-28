using System;
using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Accounts.Entities;

namespace Onion.Domain.Accounts.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(Account account, CancellationToken cancellationToken = default);
    }
}