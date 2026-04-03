using FluentValidation;
using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Accounts.Extensions;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Accounts.Entities;
using Onion.Domain.Accounts.Interfaces;
using Onion.Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Services
{
    public sealed class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAccountRequest> _createValidator;

        public AccountService(
            IAccountRepository repository, 
            IUnitOfWork unitOfWork,
            IValidator<CreateAccountRequest> createValidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
        }

        public async Task CreateAsync(CreateAccountRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);

                //result pattern vai substituir essa bomba.
                throw new ApplicationValidationException(errors);
            }

            var account = request.ToEntity(); 

            await _repository.AddAsync(account, ct);
            await _unitOfWork.CommitAsync(ct);
        }

        public async Task UpdateProfileAsync(Guid id, UpdateProfileRequest request, CancellationToken ct = default)
        {
            var account = await _repository.GetByIdAsync(id, ct);

            if (account is null)
                throw new Exception("Account not found");

            account.UpdateProfile(request.Name, request.PhotoUrl);

            await _unitOfWork.CommitAsync(ct);
        }
    }
}
