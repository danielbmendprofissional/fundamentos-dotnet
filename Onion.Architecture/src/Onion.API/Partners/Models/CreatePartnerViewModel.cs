namespace Onion.API.Partners.Models
{
    public sealed record CreatePartnerViewModel(
        string FullName,
        string Name,
        string Email,
        int PartnerType,
        string ExternalPartnerId,
        string? Image);
}
