using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.DTOs.Responses;
using Onion.Application.Common.Base;
using System;
using System.Threading;

namespace Onion.Application.Partners.Interfaces
{
    public interface IPartnerQueryRepository
    {
        Task<PartnerDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default);

        Task<PagedResult<PartnerSummary>> GetFilteredListAsync(
            Guid accountId,
            PartnerFilter filter,
            CancellationToken ct = default);
    }
}
