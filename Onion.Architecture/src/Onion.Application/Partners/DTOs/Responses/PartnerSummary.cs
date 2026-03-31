using System;

namespace Onion.Application.Partners.DTOs.Responses
{
    public sealed record PartnerSummary(
        Guid Id,
        string Name,
        string PartnerType,
        string PartnerId);
}
