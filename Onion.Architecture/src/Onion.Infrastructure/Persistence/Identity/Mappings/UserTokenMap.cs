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
    internal sealed class UserTokenMap : IEntityTypeConfiguration<IdentityUserToken<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
        {
            builder.ToTable("UserTokens");

            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            builder.Property(ut => ut.LoginProvider).HasMaxLength(128);
            builder.Property(ut => ut.Name).HasMaxLength(128);
            builder.Property(ut => ut.Value).HasMaxLength(2048);
        }
    }
}
