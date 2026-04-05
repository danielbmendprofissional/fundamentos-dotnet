using Onion.Application.Common.Base;
using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Partners.Interfaces
{
    public interface IPartnerService
    {
        Task CreateAsync(CreatePartnerRequest request, CancellationToken ct = default);
        Task UpdateAsync(UpdatePartnerRequest request, CancellationToken ct = default);
        Task<PartnerDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<PartnerSummary>> GetFilteredListAsync(PartnerFilter filter, CancellationToken ct = default);
    }
}
