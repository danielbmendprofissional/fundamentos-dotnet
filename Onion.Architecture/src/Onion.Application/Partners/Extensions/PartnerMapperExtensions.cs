using Onion.Application.Common.Extensions;
using Onion.Application.Partners.DTOs.Requests;
using Onion.Domain.Common.ValueObjects;
using Onion.Domain.Partners.Entities;
using Onion.Domain.Partners.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Partners.Extensions
{
    public static class PartnerMapperExtensions
    {
        public static Partner ToEntity(this CreatePartnerRequest request)
            => Partner.Create(
                request.FullName.ToValueObject(),
                request.Name,
                request.Email.ToValueObject(),
                (PartnerType)request.PartnerType,
                request.PartnerId,
                request.Image);
    }
}
