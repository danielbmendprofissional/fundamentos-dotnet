using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record UpdateAppointmentRequest(
        Guid Id,
        string Name,
        int AppointmentType,
        DateOnly StartDate,
        DateOnly EndDate,
        AddressRequest Address,
        string? Image,
        string? Description);
}