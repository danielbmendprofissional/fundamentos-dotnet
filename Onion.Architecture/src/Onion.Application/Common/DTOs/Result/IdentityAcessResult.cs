using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.DTOs.Result
{
    public record IdentityAcessResult(
        bool Succeeded, 
        Guid UserId = default, 
        IEnumerable<string>? Errors = null);
}
