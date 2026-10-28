using Onion.API.Accounts.Models;
using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Common.DTOs.Requests;

namespace Onion.API.Accounts.Extensions
{
    public static class AccountMappingExtensions
    {
        public static CreateAccountRequest ToRequest(this CreateAccountViewModel viewModel)
            => new(
                new EmailRequest(viewModel.Email),
                viewModel.Password,
                new FullNameRequest(viewModel.FullName),
                viewModel.PhotoUrl);

        public static LoginRequest ToRequest(this LoginViewModel viewModel)
            => new(
                viewModel.Email,
                viewModel.Password,
                viewModel.TwoFactorCode,
                viewModel.TwoFactorRecoveryCode,
                viewModel.RememberMe);

        public static UpdateProfileRequest ToRequest(this UpdateProfileViewModel viewModel)
            => new(
                new FullNameRequest(viewModel.FullName),
                viewModel.Image);
    }
}
