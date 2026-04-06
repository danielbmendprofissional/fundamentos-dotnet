using Microsoft.EntityFrameworkCore;
using Onion.Application.Accounts.DTOs.Responses;
using Onion.Application.Accounts.Interfaces;
using Onion.Domain.Accounts.Entities;
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

        public async Task<AccountDetailsResponse?> GetDetails(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.AccountDetails
                .AsNoTracking()
                .Where(x => x.Id == accountId)
                .Select(x => new AccountDetailsResponse(
                    x.Email,
                    x.FormatedFullName,
                    x.PhotoUrl,
                    x.IsActive,
                    x.CreatedAt
                    ))
                .SingleOrDefaultAsync(cancellationToken);
                            
        }

        public async Task<AccountSummaryResponse?> GetSummary(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _context.AccountDetails
                .AsNoTracking()
                .Where(x => x.Id == accountId)
                .Select(x => new AccountSummaryResponse(
                    x.FormatedFullName,
                    x.PhotoUrl
                    ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
