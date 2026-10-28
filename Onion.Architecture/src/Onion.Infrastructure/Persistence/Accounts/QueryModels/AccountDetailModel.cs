using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.QueryModels
{
    public sealed class AccountDetailsModel
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FormatedFullName { get; init; } = string.Empty;
        public string? PhotoUrl { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
