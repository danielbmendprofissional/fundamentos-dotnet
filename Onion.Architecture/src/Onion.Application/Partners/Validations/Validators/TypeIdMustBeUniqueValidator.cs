using FluentValidation;
using Onion.Application.Common.Validations;
using Onion.Application.Partners.DTOs.Validations;
using Onion.Domain.Partners.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Partners.Validations.Validators
{
    public sealed class TypeIdMustBeUniqueValidator : BaseHandlerValidator<TypeIdDTO>
    {
        public TypeIdMustBeUniqueValidator(IPartnerRepository repository)
        {
            RuleFor(x => x.TypeId)
                .MustAsync(async (id, ct) => !await repository.ExistsByPartnerIdAsync(id, ct))
                .WithMessage("This Partner identifier is already in use.");
        }
    }
}