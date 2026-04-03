using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.QueryModels
{
    public sealed class AccountSummaryModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? PhotoUrl { get; init; }
    }
}
