using FluentValidation;
using Onion.Application.Common.Base;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Interfaces;
using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.DTOs.Responses;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        private readonly IInvitationRepository _repository;
        private readonly IInvitationQueryRepository _queryRepository;
        private readonly IValidator<CreateInvitationRequest> _createValidator;

        public InvitationService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,

            IInvitationRepository repository,
            IInvitationQueryRepository queryRepository,
            IValidator<CreateInvitationRequest> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;

            _repository = repository;
            _queryRepository = queryRepository;
            _createValidator = createValidator;
        }

        public async Task CreateAsync(CreateInvitationRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var invitation = request.ToEntity();

            await _repository.AddAsync(invitation, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task AcceptAsync(Guid id, CancellationToken ct = default)
        {
            var invitation = await _repository.GetByIdAsync(id, ct);
            if (invitation is null)
                throw new ApplicationValidationException(new[] { "Invitation not found." });

            invitation.Accept();

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task RejectAsync(RespondInvitationRequest request, CancellationToken ct = default)
        {
            var invitation = await _repository.GetByIdAsync(request.Id, ct);
            if (invitation is null)
                throw new ApplicationValidationException(new[] { "Invitation not found." });

            invitation.Reject(request.Reason);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(RespondInvitationRequest request, CancellationToken ct = default)
        {
            var invitation = await _repository.GetByIdAsync(request.Id, ct);
            if (invitation is null)
                throw new ApplicationValidationException(new[] { "Invitation not found." });

            invitation.Cancel(request.Reason);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<InvitationDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();

            return await _queryRepository.GetDetailsAsync(id, accountId, ct);
        }

        public async Task<PagedResult<InvitationSummary>> GetFilteredListAsync(InvitationFilter filter, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();

            return await _queryRepository.GetFilteredListAsync(accountId, filter, ct);
        }
    }
}
