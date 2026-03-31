using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Responses
{
    public sealed record AccountDetails(
        string Email,
        string Name,
        string? PhotoUrl,
        bool IsActive,
        DateTime CreatedAt
        );
}