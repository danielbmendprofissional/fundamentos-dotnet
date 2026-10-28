using Onion.Application.Common.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Requests
{
    public sealed record CreateAccountRequest(
        EmailRequest Email,
        string Password,
        FullNameRequest FullName,
        string? PhotoUrl);
}
