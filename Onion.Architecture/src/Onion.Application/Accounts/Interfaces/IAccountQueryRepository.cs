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
        Task<AccountDetails> GetDetails(CancellationToken cancellationToken = default);
        Task<AccountSummary> GetSummary(CancellationToken cancellationToken = default);
    }
}
