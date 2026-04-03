using FluentValidation;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Interfaces;
using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.Extensions;
using Onion.Application.Partners.Interfaces;
using Onion.Domain.Partners.Interfaces;
using Onion.Domain.Partners.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Partners.Services
{
    public sealed class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreatePartnerRequest> _createValidator;

        public PartnerService(
            IPartnerRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<CreatePartnerRequest> createValidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
        }

        public async Task CreateAsync(CreatePartnerRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var partner = request.ToEntity();

            await _repository.AddAsync(partner, ct);
            await _unitOfWork.CommitAsync(ct);
        }

        public async Task UpdateAsync(UpdatePartnerRequest request, CancellationToken ct = default)
        {
            var partner = await _repository.GetByIdAsync(request.Id, ct);
            if (partner is null)
                throw new ApplicationValidationException(new[] { "Partner not found." });

            partner.Update(
                FullName.Create(request.FullName),
                request.Name,
                request.Image
            );

            await _unitOfWork.CommitAsync(ct);
        }
    }
}
