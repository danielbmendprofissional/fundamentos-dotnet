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
    public sealed class AccountMustExistValidator : AbstractValidator<AccountIdDTO>
    {
        private readonly IAccountRepository _repository;

        public AccountMustExistValidator(IAccountRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) => await _repository.GetByIdAsync(id, ct) != null)
                .WithMessage("The specified account does not exist.");
        }
    }
}
