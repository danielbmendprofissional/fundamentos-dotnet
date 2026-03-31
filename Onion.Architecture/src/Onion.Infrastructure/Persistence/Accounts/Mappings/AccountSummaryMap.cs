using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Accounts.QueryModels;

namespace Onion.Infrastructure.Persistence.Accounts.Mappings
{
    internal class AccountSummaryMap
        : IEntityTypeConfiguration<AccountSummaryModel>
    {
        public void Configure(EntityTypeBuilder<AccountSummaryModel> builder)
        {
            builder.ToTable("Account");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name);
            builder.Property(p => p.PhotoUrl);

        }
    }
}
