using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Invitations.DTOs.Requests
{
    public sealed record InvitationFilter(
        string? RecipientEmail,
        int? Status,
        int? Type) : Filter;
}
