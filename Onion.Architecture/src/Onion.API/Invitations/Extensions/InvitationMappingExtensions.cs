using Onion.API.Invitations.Models;
using Onion.Application.Invitations.DTOs.Requests;

namespace Onion.API.Invitations.Extensions
{
    public static class InvitationMappingExtensions
    {
        public static CreateInvitationRequest ToRequest(this CreateInvitationViewModel viewModel)
            => new(
                viewModel.AccountId,
                viewModel.RecipientEmail,
                viewModel.Type,
                viewModel.TargetId,
                viewModel.ExpiresAt,
                viewModel.Note);

        public static InvitationFilter ToRequest(this InvitationFilterViewModel viewModel)
            => new(
                viewModel.Page,
                viewModel.PageSize,
                viewModel.Skip,
                viewModel.Take,
                viewModel.RecipientEmail,
                viewModel.Status,
                viewModel.Type);

        public static RespondInvitationRequest ToRequest(this RespondInvitationViewModel viewModel, Guid id)
            => new(id, viewModel.Reason);
    }
}
