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
        Task<AppointmentDetails?> GetDetailsAsync(Guid id, Guid accountId, CancellationToken cancellationToken = default);

        Task<PagedResult<AppointmentSummary>> GetFilteredListAsync(
            Guid accountId, 
            AppointmentFilter filter, 
            CancellationToken cancellationToken = default);

        Task<AddressDetails?> GetAddressDetailsAsync(
            Guid id, 
            Guid accountId, 
            CancellationToken ct);

        Task<AppointmentDateDetails?> GetAppointmentDateDetailsAsync(
            Guid id, 
            Guid dateId, 
            Guid accountId, 
            CancellationToken ct);

        Task<ParticipantDetails?> GetParticipantDetailsAsync(
            Guid id, 
            Guid participantId, 
            Guid accountId, 
            CancellationToken ct);
    }
}
