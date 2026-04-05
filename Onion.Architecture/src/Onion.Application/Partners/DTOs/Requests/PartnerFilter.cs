using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Partners.DTOs.Requests
{
    public sealed record PartnerFilter(
        int Page,
        int PageSize,
        int Skip,
        int Take,
        string? Name,
        string? Email,
        int? PartnerType) : Filter(Page, PageSize, Skip, Take);
}
