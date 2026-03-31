using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record AppointmentDetails(
        Guid Id,
        string Name,
        string? Description,
        string? Image,
        string AppointmentTypeName,
        DateOnly StartDate,
        DateOnly EndDate,
        AddressSummary Address,
        IEnumerable<AppointmentDateSummary> Dates,
        IEnumerable<ParticipantSummary> Participants);
}
