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
    public interface IAppointmentService
    {
        Task CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default);
        Task UpdateAsync(UpdateAppointmentRequest request, CancellationToken ct = default);

        Task<PagedResult<AppointmentSummary>> GetFilteredListAsync(AppointmentFilter filter, CancellationToken ct = default);
        Task<AppointmentDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default);
        Task<AddressDetails?> GetAddressDetailsAsync(Guid id, CancellationToken ct = default);
        Task<AppointmentDateDetails?> GetAppointmentDateDetailsAsync(Guid id, Guid appointmentDateId, CancellationToken ct = default);
        Task<ParticipantDetails?> GetParticipantDetailsAsync(Guid id, Guid participantId, CancellationToken ct = default);
    }
}
