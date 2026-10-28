using Microsoft.EntityFrameworkCore;
using Onion.Domain.Invitations.Entities;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Invitations.Repositories
{
    internal sealed class InvitationRepository : IInvitationRepository
    {
        private readonly CalendarContext _context;

        public InvitationRepository(CalendarContext context) => _context = context;

        public async Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Invitations.FirstOrDefaultAsync(x => x.Id == id, ct);
        // colocar where pelo usuário Id também.

        public async Task AddAsync(Invitation invitation, CancellationToken ct = default)
            => await _context.Invitations.AddAsync(invitation, ct);
        // colocar where pelo usuário Id também.

        public void Update(Invitation invitation) => _context.Invitations.Update(invitation);
    }
}
