using Microsoft.EntityFrameworkCore;
using Onion.Domain.Partners.Entities;
using Onion.Domain.Partners.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.Repositories
{
    internal sealed class PartnerRepository : IPartnerRepository
    {
        private readonly CalendarContext _context;
        public PartnerRepository(CalendarContext context) => _context = context;

        public async Task<Partner?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Partners.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(Partner partner, CancellationToken ct = default)
            => await _context.Partners.AddAsync(partner, ct);

        public void Update(Partner partner) => _context.Partners.Update(partner);

        public async Task<bool> ExistsByPartnerIdAsync(string partnerId, CancellationToken ct = default)
            => await _context.Partners.AnyAsync(x => x.TypeId == partnerId);
    }
}
