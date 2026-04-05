using Onion.API.Common;

namespace Onion.API.Appointments.Models
{
    public sealed record AppointmentFilterViewModel(
        string? Name,
        string? Description,
        long? AppointmentType,
        DateOnly? StartDate,
        DateOnly? EndDate) : FilterViewModel;
}
