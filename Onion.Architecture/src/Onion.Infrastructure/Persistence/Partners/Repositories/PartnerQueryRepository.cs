using Microsoft.EntityFrameworkCore;
using Onion.Application.Common.Base;
using Onion.Application.Partners.DTOs.Requests;
using Onion.Application.Partners.DTOs.Responses;
using Onion.Application.Partners.Interfaces;
using Onion.Domain.Partners.Enums;
using Onion.Infrastructure.Persistence.Common.Context;
using Onion.Infrastructure.Persistence.Partners.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.Repositories
{
    public sealed class PartnerQueryRepository : IPartnerQueryRepository
    {
        private readonly CalendarQueryContext _context;
        public PartnerQueryRepository(CalendarQueryContext context) => _context = context;

        public async Task<PartnerDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.PartnerDetails
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PartnerDetails(
                    x.Id,
                    x.FormatedFullName,
                    x.Name,
                    x.EmailAddress,
                    ((PartnerType)x.PartnerType).ToString(),
                    x.PartnerId,
                    x.Image
                ))
                .SingleOrDefaultAsync(ct);
        }

        public async Task<PagedResult<PartnerSummary>> GetFilteredListAsync(
            Guid accountId, 
            PartnerFilter filter, 
            CancellationToken ct = default)
        {
            var query = _context.PartnerSummaries
                .AsNoTracking()
                .Where(x => EF.Property<Guid>(x, "AccountId") == accountId);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderBy(x => x.Name)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => new PartnerSummary(
                    x.Id,
                    x.Name,
                    x.ResumName,
                    ((PartnerType)x.PartnerType).ToString()
                ))
                .ToListAsync(ct);

            return new PagedResult<PartnerSummary>(items, total, filter.Page, filter.PageSize);
        }
    }
}
