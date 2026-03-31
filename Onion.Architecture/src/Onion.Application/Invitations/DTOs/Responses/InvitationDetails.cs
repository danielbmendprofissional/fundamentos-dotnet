using System;

namespace Onion.Application.Invitations.DTOs.Responses
{
    public sealed record InvitationDetails(
        Guid Id,
        Guid OwnerId,
        string RecipientEmail,
        string Type,
        string Status,
        Guid? TargetId,
        DateTime CreatedAt,
        DateTime? ExpiresAt,
        string? Note);
}
