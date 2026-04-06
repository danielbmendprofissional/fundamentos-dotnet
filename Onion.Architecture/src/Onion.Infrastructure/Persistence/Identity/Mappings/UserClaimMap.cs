using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Identity.Mappings
{
    internal sealed class UserClaimMap : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
        {
            builder.ToTable("UserClaims");

            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.ClaimType).HasMaxLength(256).IsRequired();
            builder.Property(uc => uc.ClaimValue).HasMaxLength(1024);
        }
    }
}
