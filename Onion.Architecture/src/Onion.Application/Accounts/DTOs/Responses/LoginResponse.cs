using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Accounts.DTOs.Responses
{
    public record LoginResponse
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public bool RequiresTwoFactor { get; init; }
        public string? Message { get; init; }

        public LoginResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            RequiresTwoFactor = false;
        }

        public LoginResponse() { }

        public static LoginResponse TwoFactor(string message = "Autenticação de dois fatores necessária.")
            => new() { RequiresTwoFactor = true, Message = message };
    }
}
