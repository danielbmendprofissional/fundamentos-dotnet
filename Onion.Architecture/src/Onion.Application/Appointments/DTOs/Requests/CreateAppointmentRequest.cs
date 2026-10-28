using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record CreateAppointmentRequest(
        string Name,
        string? Description,
        string? Image,
        int AppointmentType,
        DateOnly StartDate,
        DateOnly EndDate,
        AddressRequest Address,
        List<AppointmentDateRequest> Dates,
        List<ParticipantRequest> Participants
        );
}