using Microsoft.EntityFrameworkCore;
using Onion.Domain.Accounts.Entities;
using Onion.Domain.Accounts.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;


namespace Onion.Infrastructure.Persistence.Accounts.Repositories
{
    internal sealed class AccountRepository : IAccountRepository
    {
        private readonly CalendarContext _context;

        public AccountRepository(CalendarContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Accounts.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
            => await _context.Accounts.AddAsync(account, cancellationToken);
    }
}
