using Microsoft.EntityFrameworkCore;
using Onion.Application.Common.Base;
using Onion.Application.Invitations.DTOs.Requests;
using Onion.Application.Invitations.DTOs.Responses;
using Onion.Application.Invitations.Interfaces;
using Onion.Domain.Invitations.Enums;
using Onion.Infrastructure.Persistence.Common.Context;
using Onion.Infrastructure.Persistence.Invitations.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Invitations.Repositories
{
    public sealed class InvitationQueryRepository : IInvitationQueryRepository
    {
        private readonly CalendarQueryContext _context;

        public InvitationQueryRepository(CalendarQueryContext context) => _context = context;

        public async Task<InvitationDetails?> GetDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.InvitationDetails
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new InvitationDetails(
                    x.Id,
                    ((InvitationType)x.Type).ToString(),
                    ((InvitationStatus)x.Status).ToString(),
                    x.RecipientEmail,
                    x.CreatedAt,
                    x.ExpiresAt,
                    x.RespondedAt,
                    x.TargetId,
                    x.Note
                ))
                .SingleOrDefaultAsync(ct);
        }

        public async Task<InvitationDetails?> GetDetailsAsync(Guid id, Guid accountId, CancellationToken ct = default)
        {
            return await GetDetailsAsync(id, ct);
        }

        public async Task<PagedResult<InvitationSummary>> GetFilteredListAsync(
            Guid accountId, 
            InvitationFilter filter, 
            CancellationToken ct = default)
        {
            var query = _context.InvitationSummaries
                .AsNoTracking()
                .Where(x => EF.Property<Guid>(x, "AccountId") == accountId);

            if (!string.IsNullOrWhiteSpace(filter.RecipientEmail))
                query = query.Where(x => x.RecipientEmail.Contains(filter.RecipientEmail));

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => new InvitationSummary(
                    x.Id,
                    ((InvitationType)x.Type).ToString(),
                    ((InvitationStatus)x.Status).ToString(),
                    x.RecipientEmail,
                    x.CreatedAt,
                    x.ExpiresAt
                ))
                .ToListAsync(ct);

            return new PagedResult<InvitationSummary>(items, total, filter.Page, filter.PageSize);
        }
    }
}
