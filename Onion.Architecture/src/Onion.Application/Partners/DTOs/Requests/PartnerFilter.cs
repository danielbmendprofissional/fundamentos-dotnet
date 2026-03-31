using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Partners.DTOs.Requests
{
    public sealed record PartnerFilter(
        Guid? OwnerId,
        string? Search) : Filter;
}
