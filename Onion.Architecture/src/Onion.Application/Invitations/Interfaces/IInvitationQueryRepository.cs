using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.DTOs.Responses;
using Onion.Application.Common.Base;
using System;
using System.Threading;

namespace Onion.Application.Invitations.Interfaces
{
    public interface IInvitationQueryRepository
    {
        Task<InvitationDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default);

        Task<PagedResult<InvitationSummary>> GetFilteredListAsync(
            Guid accountId,
            InvitationFilter filter,
            CancellationToken ct = default);
    }
}
