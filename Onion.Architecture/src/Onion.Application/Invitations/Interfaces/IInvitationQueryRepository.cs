using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.DTOs.Responses;
using Onion.Application.Common.Base;
using System;
using System.Threading;

namespace Onion.Application.Invitations.Interfaces
{
    public interface IInvitationQueryRepository
    {
        Task<InvitationDetails> GetById(Guid id, CancellationToken cancellationToken = default);

        Task<PagedResult<InvitationSummary>> GetAsync(
            InvitationFilter? filter,
            CancellationToken cancellationToken = default);
    }
}
