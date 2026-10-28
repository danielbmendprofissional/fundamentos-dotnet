namespace Onion.API.Invitations.Models
{
    public sealed record CreateInvitationViewModel(
        Guid AccountId,
        string RecipientEmail,
        int Type,
        Guid TargetId,
        DateTime? ExpiresAt,
        string? Note);
}
