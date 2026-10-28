namespace Onion.API.Accounts.Models
{
    public sealed record CreateAccountViewModel(
        string FullName,
        string Email,
        string Password,
        string? PhotoUrl);
}
