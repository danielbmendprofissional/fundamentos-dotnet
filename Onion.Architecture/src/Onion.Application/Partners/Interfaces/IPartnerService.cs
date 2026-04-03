using Onion.Application.Partners.DTOs.Requests;
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
    }
}
