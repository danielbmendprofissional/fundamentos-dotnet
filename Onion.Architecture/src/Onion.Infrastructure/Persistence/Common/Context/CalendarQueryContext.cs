using Microsoft.EntityFrameworkCore;
using Onion.Infrastructure.Persistence.Accounts.Mappings;
using Onion.Infrastructure.Persistence.Accounts.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Common.Context
{
    public sealed class CalendarQueryContext : DbContext
    {
        public DbSet<AccountDetailsModel> AccountDetails => Set<AccountDetailsModel>();
        public DbSet<AccountSummaryModel> AccountSummaries => Set<AccountSummaryModel>();

        public CalendarQueryContext(DbContextOptions<CalendarQueryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountDetailsMap());
            modelBuilder.ApplyConfiguration(new AccountSummaryMap());
        }
    }
}
