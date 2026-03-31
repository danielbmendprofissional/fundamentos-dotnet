using Onion.Application.Appointments.DTOs.Requests;
using Onion.Application.Appointments.DTOs.Responses;
using Onion.Application.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.Interfaces
{
    public interface IAppointmentQueryRepository
    {
        Task<AppointmentDetails> GetById(Guid id, CancellationToken cancellationToken = default);

        Task<PagedResult<AppointmentSummary>> GetAsync(
            AppointmentFilter? filter,
            CancellationToken cancellationToken = default);
    }
}
