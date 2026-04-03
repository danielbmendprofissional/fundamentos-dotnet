using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record AppointmentDateDetails(
        Guid Id,
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate);
}
