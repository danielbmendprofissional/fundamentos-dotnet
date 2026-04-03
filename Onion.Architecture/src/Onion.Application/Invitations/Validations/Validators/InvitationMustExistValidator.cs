using FluentValidation;
using Onion.Application.Common.Validations;
using Onion.Application.Invitations.DTOs.Validation;

namespace Onion.Application.Invitations.Validations.Validators
{
    public sealed class InvitationMustExistValidator : BaseHandlerValidator<InvitationIdDTO>
    {
        public InvitationMustExistValidator(IInvitationRepository repository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) => await repository.GetByIdAsync(id, ct) != null)
                .WithMessage("Invitation not found.");
        }
    }
}
