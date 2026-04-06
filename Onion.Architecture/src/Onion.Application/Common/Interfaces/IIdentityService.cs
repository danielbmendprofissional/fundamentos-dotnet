using Onion.Application.Common.DTOs.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityAcessResult> RegisterAsync(string email, string password);

        Task<LoginResult> LoginAsync(
            string email,
            string password,
            bool rememberMe);

        Task LogoutAsync(Guid userId);

        Task<bool> ValidateTokenVersionAsync(Guid userId, int versionFromToken);
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task<bool> EnableTwoFactorAsync(Guid userId, string provider);
    }
}
