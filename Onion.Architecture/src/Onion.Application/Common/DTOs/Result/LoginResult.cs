using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.DTOs.Result
{
    public record LoginResult(
        bool Succeeded,
        bool RequiresTwoFactor = false,
        IEnumerable<string>? Errors = null,
        UserResult? User = null);
}
