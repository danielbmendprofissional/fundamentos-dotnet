using Onion.API.Accounts.Models;
using Onion.Application.Accounts.DTOs.Requests;

namespace Onion.API.Accounts.Extensions
{
    public static class AccountMappingExtensions
    {
        public static CreateAccountRequest ToRequest(this CreateAccountViewModel viewModel)
            => new(
                viewModel.Email,
                viewModel.Name,
                viewModel.PhotoUrl);

        public static UpdateProfileRequest ToRequest(this UpdateProfileViewModel viewModel)
            => new(
                viewModel.Name,
                viewModel.Image);
    }
}
