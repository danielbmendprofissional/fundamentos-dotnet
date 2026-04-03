using FluentValidation;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Interfaces;
using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.Extensions;
using Onion.Application.Invitations.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.Services
{
    public sealed class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateInvitationRequest> _createValidator;

        public InvitationService(
            IInvitationRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateInvitationRequest> createValidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
        }

        public async Task CreateAsync(CreateInvitationRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var invitation = request.ToEntity();

            await _repository.AddAsync(invitation, ct);
            await _unitOfWork.CommitAsync(ct);
        }

        public async Task AcceptAsync(Guid id, CancellationToken ct = default)
        {
            var invitation = await _repository.GetByIdAsync(id, ct);
            if (invitation is null)
                throw new ApplicationValidationException(new[] { "Invitation not found." });

            invitation.Accept();

            await _unitOfWork.CommitAsync(ct);
        }

        public async Task RejectAsync(RespondInvitationRequest request, CancellationToken ct = default)
        {
            var invitation = await _repository.GetByIdAsync(request.Id, ct);
            if (invitation is null)
                throw new ApplicationValidationException(new[] { "Invitation not found." });

            invitation.Reject(request.Reason);

            await _unitOfWork.CommitAsync(ct);
        }
    }
}
