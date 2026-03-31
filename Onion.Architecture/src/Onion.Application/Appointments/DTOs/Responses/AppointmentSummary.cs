using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record AppointmentSummary(
        Guid Id,
        string Name,
        string? Description,
        string? Image,
        string AppointmentTypeName);
}
