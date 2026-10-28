using Onion.API.Common;

namespace Onion.API.Partners.Models
{
    public sealed record PartnerFilterViewModel(
        string? Name,
        string? Email,
        int? PartnerType) : FilterViewModel;
}
