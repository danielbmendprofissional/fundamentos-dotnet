using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.DTOs.Requests
{
    public sealed record RespondInvitationRequest(
        Guid Id,
        string? Reason);
}
