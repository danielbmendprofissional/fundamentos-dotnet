using Microsoft.EntityFrameworkCore;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Worker.Model;

namespace Valuto.AuditTrail.Worker.Persistence
{
    public class AuditDbContext : DbContext
    {
        public DbSet<AuditEntry> AuditEntry => Set<AuditEntry>();
        public DbSet<AuditLogs> AuditLogs => Set<AuditLogs>();
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuditEntryMap());
            modelBuilder.ApplyConfiguration(new AuditLogsMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
