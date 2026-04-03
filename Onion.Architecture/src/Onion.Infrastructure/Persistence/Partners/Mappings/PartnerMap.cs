using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Partners.Entities;
using Onion.Infrastructure.Persistence.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.Mappings
{
    internal sealed class PartnerMap : EntityMap<Partner>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Partner> builder)
        {
            builder.ToTable("Partners");

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

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PartnerType).HasConversion<int>().IsRequired();
            builder.Property(p => p.PartnerId).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Image).HasMaxLength(500);

            builder.Property<Guid>("AccountId").IsRequired();
        }
    }
}
