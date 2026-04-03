using Onion.Application.Appointments.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Interfaces
{
    public interface IAppointmentService
    {
        Task CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default);
        Task UpdateAsync(UpdateAppointmentRequest request, CancellationToken ct = default);
    }
}
