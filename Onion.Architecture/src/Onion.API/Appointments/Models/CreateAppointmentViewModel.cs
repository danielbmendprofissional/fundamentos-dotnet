

namespace Onion.API.Appointments.Models
{
    public sealed record CreateAppointmentViewModel(
        string Name,
        string? Description,
        string? Image,
        int AppointmentType,
        DateOnly StartDate,
        DateOnly EndDate,
        AddressViewModel Address,
        List<AppointmentDateViewModel> Dates,
        List<ParticipantViewModel> Participants
        );
}
