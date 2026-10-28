using FluentValidation;
using Onion.Application.Common.Validations;
using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.DTOs.Validations;
using Onion.Application.Partners.Validations.Validators;
using Onion.Domain.Partners.Enums;
using Onion.Domain.Partners.Interfaces;

namespace Onion.Application.Partners.Validations.Handlers
{
    public sealed class CreatePartnerHandlerValidator : BaseHandlerValidator<CreatePartnerRequest>
    {
        public CreatePartnerHandlerValidator(IPartnerRepository partnerRepository)
        {
            RequiredRule(x => x.FullName);
            RequiredRule(x => x.Name);
            RequiredRule(x => x.Email);
            RequiredRule(x => x.PartnerId);

            RuleFor(x => x.Email.Address).EmailAddress();

            RuleFor(x => new TypeIdDTO(x.PartnerId))
                .SetValidator(new TypeIdMustBeUniqueValidator(partnerRepository));

            RuleFor(x => x)
                .Must(x => x.PartnerType != (int)PartnerType.Calendar || Guid.TryParse(x.PartnerId, out _))
                .WithMessage("PartnerId must be a valid GUID for Calendar type.");
        }
    }
}
