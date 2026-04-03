using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Requests
{
    public sealed record UpdateProfileRequest(
        string Name,
        string? PhotoUrl);
}
