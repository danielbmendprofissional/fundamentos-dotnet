using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Requests
{
    public record LoginRequest(
        string Email,
        string Password,
        string TwoFactorCode,
        string TwoFactorRecoveryCode,
        bool RememberMe = false);
}
