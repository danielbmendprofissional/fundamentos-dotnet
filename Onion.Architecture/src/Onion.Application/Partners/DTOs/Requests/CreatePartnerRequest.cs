using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Partners.DTOs.Requests
{
    public sealed record CreatePartnerRequest(
        string FullName,
        string Name,
        EmailRequest Email,
        int PartnerType,
        string PartnerId,
        string? Image);
}
