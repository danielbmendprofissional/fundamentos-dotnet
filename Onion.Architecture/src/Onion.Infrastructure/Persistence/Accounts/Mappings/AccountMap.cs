using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Accounts.Entities;
using Onion.Infrastructure.Persistence.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.Mappings
{
    internal sealed class AccountMap : EntityMap<Account>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.OwnsOne(p => p.FullName, fn =>
            {
                fn.Property(f => f.Name).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
                fn.Property(f => f.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
                fn.Property(f => f.FormatedFullName).HasColumnName("FullName").IsRequired().HasMaxLength(200);
                fn.Property(f => f.ResumName).HasColumnName("ResumName").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(p => p.Email, e =>
            {
                e.Property(a => a.Address).HasColumnName("Email").IsRequired().HasMaxLength(150);
            });

            builder.Property(p => p.PhotoUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
