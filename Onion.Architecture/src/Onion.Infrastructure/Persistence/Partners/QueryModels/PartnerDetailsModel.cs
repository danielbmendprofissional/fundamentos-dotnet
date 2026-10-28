using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.QueryModels
{
    public sealed class PartnerDetailsModel
    {
        public Guid Id { get; init; }
        public string FormatedFullName { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string EmailAddress { get; init; } = null!;
        public int PartnerType { get; init; }
        public string PartnerId { get; init; } = null!;
        public string? Image { get; init; }
    }
}
