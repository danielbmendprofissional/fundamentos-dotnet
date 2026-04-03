using FluentValidation;
using Onion.Application.Accounts.DTOs.Validations;
using Onion.Application.Accounts.Validations.Validators;
using Onion.Application.Common.Validations;
using Onion.Application.Invitations.DTOs.Requests;
using Onion.Domain.Accounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.Validations.Handlers
{
    public sealed class CreateInvitationHandlerValidator : BaseHandlerValidator<CreateInvitationRequest>
    {
        public CreateInvitationHandlerValidator(IAccountRepository accountRepository)
        {
            RequiredRule(x => x.RecipientEmail);
            RuleFor(x => x.RecipientEmail).EmailAddress();

            RuleFor(x => new AccountIdDTO(x.AccountId))
                .SetValidator(new AccountMustExistValidator(accountRepository));

            RuleFor(x => x.ExpiresAt)
                .Must(x => x > DateTime.UtcNow)
                .When(x => x.ExpiresAt.HasValue)
                .WithMessage("Expiration date must be in the future.");
        }
    }
}
