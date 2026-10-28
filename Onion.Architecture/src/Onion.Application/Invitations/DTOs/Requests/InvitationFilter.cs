using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Invitations.DTOs.Requests
{
    public sealed record InvitationFilter(
        int Page,
        int PageSize,
        int Skip,
        int Take,
        string? RecipientEmail,
        int? Status,
        int? Type
    ) : Filter(Page, PageSize, Skip, Take);
}
