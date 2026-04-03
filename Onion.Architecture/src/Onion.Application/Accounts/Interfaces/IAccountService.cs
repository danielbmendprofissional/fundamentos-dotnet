using Onion.Application.Accounts.DTOs.Requests;
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
        Task UpdateProfileAsync(Guid id, UpdateProfileRequest request, CancellationToken ct = default);
    }
}
