using Microsoft.AspNetCore.Identity;

namespace Onion.Infrastructure.Persistence.Identity.Entities
{
    public sealed class User : IdentityUser<Guid>
    {
        public int TokenVersion { get; set; } = 1;
    }
}
