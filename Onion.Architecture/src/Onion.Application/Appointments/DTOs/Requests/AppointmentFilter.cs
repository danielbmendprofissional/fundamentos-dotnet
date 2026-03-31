using Onion.Application.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record AppointmentFilter(
        string? Name,
        string? Description,
        long? AppointmentType,
        DateOnly? StartDate,
        DateOnly? EndDate) : Filter;
}
