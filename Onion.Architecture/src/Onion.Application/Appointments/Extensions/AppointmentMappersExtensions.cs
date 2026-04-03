using Onion.Application.Appointments.DTOs.Requests;
using Onion.Domain.Appointments.Entities;
using Onion.Domain.Appointments.Enums;
using Onion.Domain.Appointments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Extensions
{
    public static class AppointmentMappersExtensions
    {
        public static Appointment ToEntity(this CreateAppointmentRequest request)
            => Appointment.Create(
                Guid.NewGuid(), 
                request.Name, 
                (AppointmentType)request.AppointmentType, 
                request.StartDate, 
                request.EndDate, 
                request.Address.ToValueObject(),
                request.Description, 
                request.Image);

        public static Address ToValueObject(this AddressRequest request)
            => Address.Create(
                request.PostalCode,
                request.Street,
                request.Number,
                request.City,
                request.State,
                request.Country,
                request.Complement);
    }
}
