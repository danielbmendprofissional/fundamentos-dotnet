using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.DTOs.Requests
{
    public sealed record CreateInvitationRequest(
        Guid AccountId,
        string RecipientEmail,
        int Type,
        Guid TargetId,
        DateTime? ExpiresAt,
        string? Note);
}
