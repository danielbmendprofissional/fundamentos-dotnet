using Microsoft.EntityFrameworkCore;
using Onion.Domain.Appointments.Entities;
using Onion.Domain.Appointments.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.Repositories
{
    internal sealed class AppointmentRepository : IAppointmentRepository
    {
        private readonly CalendarContext _context;

        public AppointmentRepository(CalendarContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Appointments
                .Include(x => x.Dates)
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        // colocar where pelo usuário Id também.
        public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
            => await _context.Appointments.AddAsync(appointment, cancellationToken);
        // colocar where pelo usuário Id também.
        public void Update(Appointment appointment)
            => _context.Appointments.Update(appointment);
    }
}
