using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.QueryModels
{
    public sealed class AccountDetailsModel
    {
        public Guid Id { get; }
        public string Email { get; } = string.Empty;
        public string Name { get; } = string.Empty;
        public string? PhotoUrl { get; }
        public bool IsActive { get; }
        public DateTime CreatedAt { get; }
    }
}
