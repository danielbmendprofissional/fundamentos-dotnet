namespace Onion.API.Appointments.Models
{
    public sealed record AppointmentDateViewModel(
        string Name,
        DateTime StartDate,
        DateTime EndDate);
}
