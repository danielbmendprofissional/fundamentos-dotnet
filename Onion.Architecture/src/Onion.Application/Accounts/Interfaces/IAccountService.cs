using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Accounts.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Interfaces
{
    public interface IAccountService
    {
        Task CreateAsync(CreateAccountRequest request, CancellationToken ct = default);
        Task UpdateProfileAsync(UpdateProfileRequest request, CancellationToken ct = default);
        Task<AccountSummary?> GetSummaryAsync(CancellationToken ct = default);
        Task<AccountDetails?> GetDetailsAsync(CancellationToken ct = default);
    }
}
