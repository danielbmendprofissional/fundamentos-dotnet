using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Common
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly CalendarContext _context;

        public UnitOfWork(CalendarContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct) > 0;
        }
    }
}
