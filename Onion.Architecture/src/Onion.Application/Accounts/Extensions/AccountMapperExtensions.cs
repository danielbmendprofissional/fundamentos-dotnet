using Onion.Application.Accounts.DTOs.Requests;
using Onion.Domain.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.Extensions
{
    public static class AccountMapperExtensions
    {
        public static Account ToEntity(this CreateAccountRequest request)
        {
            return Account.Create(
                request.Id,
                request.Email,
                request.Name,
                request.PhotoUrl
            );
        }

        // Podemos ter mappers para transformar a entidade no DTO de resposta se necessário
    }
}
