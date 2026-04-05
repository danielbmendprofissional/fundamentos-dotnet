using Microsoft.EntityFrameworkCore;
using Onion.Application.Appointments.DTOs.Requests;
using Onion.Application.Appointments.DTOs.Responses;
using Onion.Application.Appointments.Interfaces;
using Onion.Application.Common.Base;
using Onion.Domain.Appointments.Enums;
using Onion.Infrastructure.Persistence.Appointments.QueryModels;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.Repositories
{
    public sealed class AppointmentQueryRepository : IAppointmentQueryRepository
    {
        private readonly CalendarQueryContext _context;

        public AppointmentQueryRepository(CalendarQueryContext context)
        {
            _context = context;
        }

        public async Task<AppointmentDetails?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.AppointmentDetails
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AppointmentDetails(
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Image,
                    x.AppointmentType.ToString(),
                    x.StartDate,
                    x.EndDate,
                    new AddressSummary(x.Id, x.Street, x.Number),
                    x.Dates.Select(d => new AppointmentDateSummary(d.Id, d.Name, d.StartDate, d.EndDate)),
                    x.Participants.Select(p => new ParticipantSummary(p.Id, p.Name))
                ))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<AppointmentSummary>> GetFilteredListAsync(
            Guid accountId, 
            AppointmentFilter filter, 
            CancellationToken cancellationToken = default)
        {
            var query = _context.AppointmentSummaries
                .AsNoTracking()
                .Where(x => EF.Property<Guid>(x, "AccountId") == accountId);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));


            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.StartDate)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .Select(x => new AppointmentSummary(
                    x.Id,
                    x.Name,
                    x.AppointmentType.ToString()
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<AppointmentSummary>(items, total, filter.Page, filter.PageSize);
        }

        public async Task<AddressDetails?> GetAddressDetailsAsync(Guid id, Guid accountId, CancellationToken ct)
        {
            return await _context.AddressDetails
                .AsNoTracking()
                .Where(a => a.AppointmentId == id)
                .Select(a => new AddressDetails(
                    a.AppointmentId,
                    a.Street,
                    a.Number,
                    a.Complement,
                    a.City,
                    a.State,
                    a.Country
                ))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ParticipantDetails?> GetParticipantDetailsAsync(Guid participantId, CancellationToken cancellationToken = default)
        {
            return await _context.ParticipantDetails
                .AsNoTracking()
                .Where(x => x.Id == participantId)
                .Select(x => new ParticipantDetails(
                    x.Id,
                    x.Name,
                    x.Email,
                    ((ParticipantSituation)x.Situation).ToString()
                ))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<AppointmentDateDetails?> GetAppointmentDateDetailsAsync(Guid dateId, CancellationToken cancellationToken = default)
        {
            return await _context.AppointmentDateDetails
                .AsNoTracking()
                .Where(x => x.Id == dateId)
                .Select(x => new AppointmentDateDetails(
                    x.Id,
                    x.Name,
                    x.Description ?? string.Empty,
                    x.StartDate,
                    x.EndDate
                ))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
