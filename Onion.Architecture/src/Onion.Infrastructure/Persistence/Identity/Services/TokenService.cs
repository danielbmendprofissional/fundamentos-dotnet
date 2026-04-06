using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Identity.Services
{
    internal sealed class TokenService : ITokenService
    {
        private readonly JWTConfigurationOptions _options;

        public TokenService(IOptions<JWTConfigurationOptions> options)
            => _options = options.Value;

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_options.AcessTokenExpirationMinutes!.Value),
                SigningCredentials = creds,
                Issuer = _options.Issuer,
                Audience = _options.Audience
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(tokenDescriptor));
        }

        public string GenerateRefreshToken() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
