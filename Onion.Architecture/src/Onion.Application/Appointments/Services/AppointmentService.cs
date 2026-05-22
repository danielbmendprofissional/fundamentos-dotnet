using FluentValidation;
using Onion.Application.Appointments.DTOs.Requests;
using Onion.Application.Appointments.DTOs.Responses;
using Onion.Application.Appointments.Extensions;
using Onion.Application.Appointments.Interfaces;
using Onion.Application.Common.Base;
using Onion.Application.Common.Exceptions;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Appointments.Entities;
using Onion.Domain.Appointments.Enums;
using Onion.Domain.Appointments.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Services
{
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        private readonly IAppointmentRepository _repository;
        private readonly IAppointmentQueryRepository _queryRepository;
        private readonly IValidator<CreateAppointmentRequest> _createValidator;

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IAppointmentRepository repository,
            IAppointmentQueryRepository queryRepository,
            IValidator<CreateAppointmentRequest> createValidator)
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;

            _repository = repository;
            _queryRepository = queryRepository;
            _createValidator = createValidator;
        }

        public async Task CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default)
        {
            var validationResult = await _createValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ApplicationValidationException(validationResult.Errors.Select(e => e.ErrorMessage));

            var appointment = request.ToEntity();

            foreach (var p in request.Participants)
            {
                appointment.AddParticipant(p.Name, p.Email, p.PartnerId);
            }

            await _repository.AddAsync(appointment, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(UpdateAppointmentRequest request, CancellationToken ct = default)
        {
            var appointment = await _repository.GetByIdAsync(request.Id, ct);

            if (appointment is null)
                throw new ApplicationValidationException(new[] { "Appointment not found." });

            appointment.Update(
                request.Name,
                (AppointmentType)request.AppointmentType,
                request.StartDate,
                request.EndDate,
                request.Address.ToValueObject(),
                request.Description,
                request.Image);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<PagedResult<AppointmentSummary>> GetFilteredListAsync(AppointmentFilter filter, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();
            return await _queryRepository.GetFilteredListAsync(accountId, filter, ct);
        }

        public async Task<AppointmentDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();
            return await _queryRepository.GetDetailsAsync(id, accountId, ct);
        }

        public async Task<AddressDetails?> GetAddressDetailsAsync(Guid id, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();
            return await _queryRepository.GetAddressDetailsAsync(id, accountId, ct);
        }

        public async Task<AppointmentDateDetails?> GetAppointmentDateDetailsAsync(Guid id, Guid appointmentDateId, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();
            return await _queryRepository.GetAppointmentDateDetailsAsync(id, appointmentDateId, accountId, ct);
        }

        public async Task<ParticipantDetails?> GetParticipantDetailsAsync(Guid id, Guid participantId, CancellationToken ct = default)
        {
            var accountId = _userProvider.GetUserId();
            return await _queryRepository.GetParticipantDetailsAsync(id, participantId, accountId, ct);
        }
    }
}
