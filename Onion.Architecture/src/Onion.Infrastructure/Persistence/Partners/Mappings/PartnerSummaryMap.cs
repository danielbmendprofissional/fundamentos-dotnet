using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Partners.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Partners.Mappings
{
    internal sealed class PartnerSummaryMap : IEntityTypeConfiguration<PartnerSummaryModel>
    {
        public void Configure(EntityTypeBuilder<PartnerSummaryModel> builder)
        {
            builder.ToTable("Partners");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name);
            builder.Property(p => p.ResumName);
            builder.Property(p => p.PartnerType);
            builder.Property(p => p.Image);

            builder.Property<Guid>("AccountId").HasColumnName("AccountId");
        }
    }
}
