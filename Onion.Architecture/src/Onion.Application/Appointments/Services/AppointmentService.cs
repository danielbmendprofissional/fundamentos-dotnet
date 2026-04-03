using FluentValidation;
using Onion.Application.Appointments.DTOs.Requests;
using Onion.Application.Appointments.Extensions;
using Onion.Application.Appointments.Interfaces;
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
        private readonly IAppointmentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAppointmentRequest> _createValidator;

        public AppointmentService(
            IAppointmentRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateAppointmentRequest> createValidator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.CommitAsync(ct);
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

            await _unitOfWork.CommitAsync(ct);
        }
    }
}
