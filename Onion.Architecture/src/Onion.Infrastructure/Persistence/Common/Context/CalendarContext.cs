using MediatR;
using Microsoft.EntityFrameworkCore;
using Onion.Domain.Accounts.Entities;
using Onion.Domain.Appointments.Entities;
using Onion.Domain.Common.Base;
using Onion.Domain.Invitations.Entities;
using Onion.Domain.Partners.Entities;
using Onion.Infrastructure.Persistence.Accounts.Mappings;
using Onion.Infrastructure.Persistence.Appointments.Mappings;
using Onion.Infrastructure.Persistence.Invitations.Mappings;
using Onion.Infrastructure.Persistence.Partners.Mappings;

namespace Onion.Infrastructure.Persistence.Common.Context
{
    public sealed class CalendarContext : DbContext
    {
        private readonly IMediator _mediator;

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<Partner> Partners => Set<Partner>();

        public CalendarContext(DbContextOptions<CalendarContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountMap());
            modelBuilder.ApplyConfiguration(new AppointmentMap());
            modelBuilder.ApplyConfiguration(new InvitationMap());
            modelBuilder.ApplyConfiguration(new PartnerMap());
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var entitiesWithEvents = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);

            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            return result;

            //aplicar Outbox

        }
    }
}
