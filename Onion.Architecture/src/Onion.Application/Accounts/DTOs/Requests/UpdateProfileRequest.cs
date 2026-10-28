using Onion.Application.Common.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Requests
{
    public sealed record UpdateProfileRequest(
        FullNameRequest FullName,
        string? PhotoUrl);
}
