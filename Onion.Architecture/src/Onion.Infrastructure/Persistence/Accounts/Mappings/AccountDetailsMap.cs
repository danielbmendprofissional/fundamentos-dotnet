using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Accounts.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Accounts.Mappings
{
    internal sealed class AccountDetailsMap
        : IEntityTypeConfiguration<AccountDetailsModel>
    {
        public void Configure(EntityTypeBuilder<AccountDetailsModel> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Email);
            builder.Property(p => p.FormatedFullName).HasColumnName("FullName");
            builder.Property(p => p.PhotoUrl);
            builder.Property(p => p.IsActive);
            builder.Property(p => p.CreatedAt);

        }
    }
}
