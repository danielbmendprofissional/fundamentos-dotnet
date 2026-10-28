using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.QueryModels
{
    public sealed class PartnerSummaryModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string ResumName { get; init; } = null!;
        public int PartnerType { get; init; }
        public string? Image { get; init; }
    }
}
