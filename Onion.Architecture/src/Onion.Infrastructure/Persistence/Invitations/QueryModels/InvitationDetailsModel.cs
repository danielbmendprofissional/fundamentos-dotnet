using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Invitations.QueryModels
{
    public sealed class InvitationDetailsModel
    {
        public Guid Id { get; init; }
        public int Type { get; init; }
        public int Status { get; init; }
        public string RecipientEmail { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public DateTime? RespondedAt { get; init; }
        public Guid? TargetId { get; init; }
        public string? Note { get; init; }
    }
}
