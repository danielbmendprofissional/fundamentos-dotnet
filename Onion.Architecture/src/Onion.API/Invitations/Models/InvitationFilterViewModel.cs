using Onion.API.Common;

namespace Onion.API.Invitations.Models
{
    public sealed record InvitationFilterViewModel(
        string? RecipientEmail,
        int? Status,
        int? Type) : FilterViewModel;
}
