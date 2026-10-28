using Onion.Application.Common.Base;
using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.Interfaces
{
    public interface IInvitationService
    {
        Task CreateAsync(CreateInvitationRequest request, CancellationToken ct = default);
        Task AcceptAsync(Guid id, CancellationToken ct = default);
        Task RejectAsync(RespondInvitationRequest request, CancellationToken ct = default);
        Task CancelAsync(RespondInvitationRequest request, CancellationToken ct = default);

        Task<InvitationDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<InvitationSummary>> GetFilteredListAsync(InvitationFilter filter, CancellationToken ct = default);
    }
}
