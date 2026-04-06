using Onion.Application.Accounts.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Interfaces
{
    public interface IAccountQueryRepository
    {
        Task<AccountDetailsResponse?> GetDetails(Guid accountId, CancellationToken cancellationToken = default);
        Task<AccountSummaryResponse?> GetSummary(Guid accountId, CancellationToken cancellationToken = default);
    }
}
