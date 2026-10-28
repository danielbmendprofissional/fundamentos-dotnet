using Onion.Application.Accounts.DTOs.Requests;
using Onion.Application.Common.Extensions;
using Onion.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Extensions
{
    public static class AccountMappingExtensions
    {
        public static Account ToEntity(this CreateAccountRequest request, Guid id)
        {
            return Account.Create(
                id,
                request.Email.ToValueObject(),
                request.FullName.ToValueObject(),
                request.PhotoUrl
            );
        }
    }
}
