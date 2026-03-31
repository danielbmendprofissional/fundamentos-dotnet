using System;

namespace Onion.Application.Invitations.DTOs.Responses
{
    public sealed record InvitationSummary(
        Guid Id,
        string RecipientEmail,
        string Type,
        string Status,
        DateTime CreatedAt);
}
