using Onion.Application.Invitations.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.Interfaces
{
    public interface IInvitationService
    {
        Task CreateAsync(CreateInvitationRequest request, CancellationToken ct = default);
        Task AcceptAsync(Guid id, CancellationToken ct = default);
        Task RejectAsync(RespondInvitationRequest request, CancellationToken ct = default);
    }
}
