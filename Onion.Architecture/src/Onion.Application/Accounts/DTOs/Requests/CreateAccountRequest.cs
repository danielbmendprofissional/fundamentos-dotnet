using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Requests
{
    public sealed record CreateAccountRequest(
        Guid Id,
        string Email,
        string Name,
        string? PhotoUrl);
}
