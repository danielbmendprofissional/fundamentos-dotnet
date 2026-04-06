using Microsoft.AspNetCore.Identity;
using Onion.Application.Common.DTOs.Result;
using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Identity.Services
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IdentityService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityAcessResult> RegisterAsync(string email, string password)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new IdentityAcessResult(Succeeded: false, Errors: errors);
            }

            return new IdentityAcessResult(Succeeded: true, UserId: user.Id);
        }

        public async Task<LoginResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
                return new LoginResult(false, RequiresTwoFactor: true);

            if (!result.Succeeded)
            {
                var error = result.IsLockedOut ? "Conta bloqueada." : "Credenciais inválidas.";
                return new LoginResult(false, Errors: new[] { error });
            }

            var user = await _userManager.FindByEmailAsync(email);

            var userDto = new UserResult(user!.Id, user.Email!, user.TokenVersion);

            return new LoginResult(true, User: userDto);
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.TokenVersion++;
                await _userManager.UpdateAsync(user);
            }
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ValidateTokenVersionAsync(Guid userId, int versionFromToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return user.TokenVersion == versionFromToken;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Enumerable.Empty<string>();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return roles;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new Exception("Usuário inexistente.");
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> EnableTwoFactorAsync(Guid userId, string provider)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.SetTwoFactorEnabledAsync(user!, true);
            return result.Succeeded;
        }
    }
}
