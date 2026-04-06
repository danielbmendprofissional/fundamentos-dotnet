using Onion.Application.Common.DTOs.Requests;
using Onion.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Common.Extensions
{
    public static class CommonMapperExtensions
    {
        public static Email ToValueObject(this EmailRequest request)
            => Email.Create(request.Address);

        public static FullName ToValueObject(this FullNameRequest request)
            => FullName.Create(request.Value);
    }
}
