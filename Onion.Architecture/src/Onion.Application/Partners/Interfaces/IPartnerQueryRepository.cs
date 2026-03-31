using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.DTOs.Responses;
using Onion.Application.Common.Base;
using System;
using System.Threading;

namespace Onion.Application.Partners.Interfaces
{
    public interface IPartnerQueryRepository
    {
        Task<PartnerDetails> GetById(Guid id, CancellationToken cancellationToken = default);

        Task<PagedResult<PartnerSummary>> GetAsync(
            PartnerFilter? filter,
            CancellationToken cancellationToken = default);
    }
}
