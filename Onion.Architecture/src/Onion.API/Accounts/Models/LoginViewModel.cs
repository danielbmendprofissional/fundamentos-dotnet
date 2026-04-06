namespace Onion.API.Accounts.Models
{
    public sealed record LoginViewModel(
        string Email,
        string Password,
        string TwoFactorCode,
        string TwoFactorRecoveryCode,
        bool RememberMe = false);
}