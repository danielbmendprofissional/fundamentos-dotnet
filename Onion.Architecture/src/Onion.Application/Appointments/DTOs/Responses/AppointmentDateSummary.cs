using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record AppointmentDateSummary(
        Guid Id,
        string Name,
        DateTime StartDate,
        DateTime EndDate);
}
