using MediatR;
using Microsoft.EntityFrameworkCore;
using Onion.Domain.Accounts.Entities;
using Onion.Domain.Common.Base;
using Onion.Infrastructure.Persistence.Accounts.Mappings;

namespace Onion.Infrastructure.Persistence.Common.Context
{
    public sealed class CalendarContext : DbContext
    {
        private readonly IMediator _mediator;

        public DbSet<Account> Accounts => Set<Account>();

        public CalendarContext(DbContextOptions<CalendarContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountMap());
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
