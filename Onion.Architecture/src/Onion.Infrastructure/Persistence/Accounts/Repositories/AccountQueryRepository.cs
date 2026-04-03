using Microsoft.EntityFrameworkCore;
using Onion.Application.Accounts.DTOs.Responses;
using Onion.Application.Accounts.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.Repositories
{
    public sealed class AccountQueryRepository : IAccountQueryRepository
    {
        private readonly CalendarQueryContext _context;

        public AccountQueryRepository(CalendarQueryContext context)
        {
            _context = context;
        }

        public async Task<AccountDetails?> GetDetails(CancellationToken cancellationToken = default)
        {
            //wait auth user

            return await _context.AccountDetails
                .AsNoTracking()
                .Where(x => x.Id == Guid.Empty)
                .Select(x => new AccountDetails(
                    x.Email,
                    x.Name,
                    x.PhotoUrl,
                    x.IsActive,
                    x.CreatedAt
                    ))
                .SingleOrDefaultAsync(cancellationToken);
                            
        }

        public async Task<AccountSummary?> GetSummary(CancellationToken cancellationToken = default)
        {
            //wait auth user

            return await _context.AccountDetails
                .AsNoTracking()
                .Where(x => x.Id == Guid.Empty)
                .Select(x => new AccountSummary(
                    x.Name,
                    x.PhotoUrl
                    ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
