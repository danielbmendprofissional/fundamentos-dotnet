using FluentValidation;
using Onion.Application.Appointments.DTOs.Validations;
using Onion.Domain.Appointments.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Validations.Validators
{
    public sealed class AppointmentMustExistValidator : AbstractValidator<AppointmentIdDTO>
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentMustExistValidator(IAppointmentRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) => await _repository.GetByIdAsync(id, ct) != null)
                .WithMessage("Appointment not found.");
        }
    }
}
