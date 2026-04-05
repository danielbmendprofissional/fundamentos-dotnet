namespace Onion.API.Appointments.Models
{
    public sealed record UpdateAppointmentViewModel(
        string Name,
        int AppointmentType,
        DateOnly StartDate,
        DateOnly EndDate,
        AddressViewModel Address,
        string? Image,
        string? Description);
}
