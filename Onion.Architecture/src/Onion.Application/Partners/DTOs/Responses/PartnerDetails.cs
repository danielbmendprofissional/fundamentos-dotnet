using System;

namespace Onion.Application.Partners.DTOs.Responses
{
    public sealed record PartnerDetails(
        Guid Id,
        string Name,
        string? FullName,
        string Email,
        string PartnerType,
        string PartnerId,
        string? Image);
}
