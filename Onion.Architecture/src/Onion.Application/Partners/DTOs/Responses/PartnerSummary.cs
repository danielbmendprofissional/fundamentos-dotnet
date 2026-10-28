using System;

namespace Onion.Application.Partners.DTOs.Responses
{
    public sealed record PartnerSummary(
        Guid Id,
        string Name,
        string ResumName,
        string PartnerType);
}
