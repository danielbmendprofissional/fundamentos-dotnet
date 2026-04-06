using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Onion.Infrastructure.Persistence.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Common.Context
{
    public sealed class CalendarUserContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public CalendarUserContext(DbContextOptions<CalendarUserContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(CalendarUserContext).Assembly);
        }
    }
}
