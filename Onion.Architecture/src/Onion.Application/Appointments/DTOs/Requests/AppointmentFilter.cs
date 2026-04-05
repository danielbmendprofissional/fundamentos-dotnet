using Onion.Application.Common.Base;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record AppointmentFilter(
        int Page,
        int PageSize,
        int Skip,
        int Take,
        string? Name,
        string? Description,
        long? AppointmentType,
        DateOnly? StartDate,
        DateOnly? EndDate)
        : Filter(Page, PageSize, Skip, Take);
}
