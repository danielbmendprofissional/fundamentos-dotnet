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

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

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
