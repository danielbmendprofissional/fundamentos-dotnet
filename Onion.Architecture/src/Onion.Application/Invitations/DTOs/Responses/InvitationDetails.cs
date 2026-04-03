using System;

namespace Onion.Application.Invitations.DTOs.Responses
{
    public sealed record InvitationDetails(
        Guid Id,
        string Type,
        string Status,
        string RecipientEmail,
        DateTime CreatedAt,
        DateTime? ExpiresAt,
        DateTime? RespondedAt,
        Guid? TargetId,
        string? Note);
}
