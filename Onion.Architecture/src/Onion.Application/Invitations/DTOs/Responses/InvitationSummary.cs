using System;

namespace Onion.Application.Invitations.DTOs.Responses
{
    public sealed record InvitationSummary(
        Guid Id,
        string Type,
        string Status,
        string RecipientEmail,
        DateTime CreatedAt,
        DateTime? ExpiresAt);
}
