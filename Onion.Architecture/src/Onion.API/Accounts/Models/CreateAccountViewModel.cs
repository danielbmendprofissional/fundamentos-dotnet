namespace Onion.API.Accounts.Models
{
    public sealed record CreateAccountViewModel(
        string Name,
        string Email,
        string Password,
        string? PhotoUrl);
}
