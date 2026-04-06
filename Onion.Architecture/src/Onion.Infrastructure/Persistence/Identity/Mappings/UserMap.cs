using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Identity.Mappings
{
    internal sealed class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("UserId");

            builder.Property(u => u.UserName).HasMaxLength(256).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();

            builder.Property(u => u.TokenVersion).IsRequired().HasDefaultValue(1);

            builder.Property(u => u.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
        }
    }
}
