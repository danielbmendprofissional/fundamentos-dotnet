using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Partners.DTOs.Requests
{
    public sealed record PartnerFilter(
        string? Name,
        string? Email,
        int? PartnerType) : Filter;
}
