namespace Onion.API.Partners.Models
{
    public sealed record UpdatePartnerViewModel(
        string FullName,
        string Name,
        string? Image);
}
