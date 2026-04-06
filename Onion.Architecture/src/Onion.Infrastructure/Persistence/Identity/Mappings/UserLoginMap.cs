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
    internal sealed class UserLoginMap : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
        {
            builder.ToTable("UserLogins");

            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            builder.Property(ul => ul.LoginProvider).HasMaxLength(128);
            builder.Property(ul => ul.ProviderKey).HasMaxLength(128);
            builder.Property(ul => ul.ProviderDisplayName).HasMaxLength(256);
        }
    }
}
