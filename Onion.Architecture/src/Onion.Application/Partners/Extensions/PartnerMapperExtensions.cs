using Onion.Application.Partners.DTOs.Requests;
using Onion.Domain.Partners.Entities;
using Onion.Domain.Partners.Enums;
using Onion.Domain.Partners.ValueObjects;
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
                FullName.Create(request.FullName),
                request.Name,
                request.Email.ToValueObject(),
                (PartnerType)request.PartnerType,
                request.PartnerId,
                request.Image);

        public static Email ToValueObject(this EmailRequest request)
            => Email.Create(request.Address);
    }
}
