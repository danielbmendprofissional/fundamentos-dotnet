using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Identity.Configuration
{
    public class JWTConfigurationOptions
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public double? AcessTokenExpirationMinutes { get; set; }
        public double? RefreshTokenExpirationMinutes { get; set; }
        public string? SecretKey { get; set; }
    }
}
