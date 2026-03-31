using Onion.Application.Common.Base;
using System;

namespace Onion.Application.Invitations.DTOs.Requests
{
    public sealed record InvitationFilter(
        Guid? OwnerId,
        string? RecipientEmail,
        int? Status) : Filter;
}
