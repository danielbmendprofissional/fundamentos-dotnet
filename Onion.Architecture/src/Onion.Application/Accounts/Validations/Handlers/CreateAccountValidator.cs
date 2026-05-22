using FluentValidation;
using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Accounts.DTOs.Validations;
using Onion.Application.Accounts.Validations.Validators;
using Onion.Application.Common.Validations;
using Onion.Domain.Accounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Validations.Handlers
{
    public sealed class CreateAccountValidator : BaseHandlerValidator<CreateAccountRequest>
    {
        public CreateAccountValidator(IAccountRepository repository)
        {
            RequiredRule(x => x.Email);
            RequiredRule(x => x.FullName);
            RuleFor(x => x.Email.Address).EmailAddress();
        }
    }
}
