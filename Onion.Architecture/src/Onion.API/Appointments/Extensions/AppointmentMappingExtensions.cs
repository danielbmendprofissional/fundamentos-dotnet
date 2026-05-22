using Onion.API.Appointments.Models;
using Onion.API.Common;
using Onion.Application.Appointments.DTOs.Requests;

namespace Onion.API.Appointments.Extensions
{
    public static class AppointmentMappingExtensions
    {
        public static CreateAppointmentRequest ToRequest(this CreateAppointmentViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel.Address);

            return new(
                viewModel.Name,
                viewModel.Description,
                viewModel.Image,
                viewModel.AppointmentType,
                viewModel.StartDate,
                viewModel.EndDate,
                viewModel.Address.ToRequest(),
                viewModel.Dates.Select(p => p.ToRequest()).ToList(),
                viewModel.Participants.Select(p => p.ToRequest()).ToList());
        }

        public static UpdateAppointmentRequest ToRequest(this UpdateAppointmentViewModel viewModel, Guid id)
        {
            ArgumentNullException.ThrowIfNull(viewModel.Address);

            return new(
                id,
                viewModel.Name,
                viewModel.AppointmentType,
                viewModel.StartDate,
                viewModel.EndDate,
                viewModel.Address.ToRequest(),
                viewModel.Image,
                viewModel.Description);
        }

        public static AppointmentFilter ToRequest(this AppointmentFilterViewModel viewModel)
            => new(
                viewModel.Page,
                viewModel.PageSize,
                viewModel.Skip,
                viewModel.Take,
                viewModel.Name,
                viewModel.Description,
                viewModel.AppointmentType,
                viewModel.StartDate,
                viewModel.EndDate);

        private static AddressRequest ToRequest(this AddressViewModel viewModel)
            => new(
                viewModel.Street, 
                viewModel.Number, 
                viewModel.Complement, 
                viewModel.City, viewModel.State, 
                viewModel.PostalCode, 
                viewModel.Country);

        private static AppointmentDateRequest ToRequest(this AppointmentDateViewModel viewModel)
            => new(
                viewModel.Name, 
                viewModel.StartDate, 
                viewModel.EndDate);

        private static ParticipantRequest ToRequest(this ParticipantViewModel viewModel)
            => new(
                viewModel.Name, 
                viewModel.Email, 
                viewModel.PartnerId);
    }
}
