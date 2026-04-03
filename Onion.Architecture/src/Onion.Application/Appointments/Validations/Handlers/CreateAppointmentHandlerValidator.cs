using FluentValidation;
using Onion.Application.Accounts.DTOs.Validations;
using Onion.Application.Accounts.Validations.Validators;
using Onion.Application.Appointments.DTOs.Requests;
using Onion.Application.Common.Validations;
using Onion.Domain.Accounts.Interfaces;
using Onion.Domain.Partners.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Validations.Handlers
{
    public sealed class CreateAppointmentHandlerValidator : BaseHandlerValidator<CreateAppointmentRequest>
    {
        public CreateAppointmentHandlerValidator(
            IAccountRepository accountRepository,
            IPartnerRepository partnerRepository)
        {
            RequiredRule(x => x.Name);
            MaxLengthRule(x => x.Name, 150);

            RuleFor(x => x)
                .Must(x => x.EndDate > x.StartDate)
                .WithMessage("The end date must be after the start date.");

            RuleForEach(x => x.Participants).ChildRules(participant =>
            {
                participant.RuleFor(p => p.PartnerId).NotEmpty();
            });
        }
    }
}
