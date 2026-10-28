using FluentValidation;
using Onion.Application.Accounts.DTOs.Validations;
using Onion.Application.Common.Validations;
using Onion.Domain.Accounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Validations.Validators
{
    public sealed class AccountMustNotExistValidator : AbstractValidator<AccountIdDTO>
    {
        private readonly IAccountRepository _repository;

        public AccountMustNotExistValidator(IAccountRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) => await _repository.GetByIdAsync(id, ct) == null)
                .WithMessage("This Account ID is already registered.");
        }
    }
}
