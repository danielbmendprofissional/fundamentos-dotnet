using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record AppointmentDateRequest(
        string Name,
        DateTime StartDate,
        DateTime EndDate
        );
}
