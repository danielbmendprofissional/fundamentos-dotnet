using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Invitations.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Invitations.Mappings
{
    internal sealed class InvitationSummaryMap : IEntityTypeConfiguration<InvitationSummaryModel>
    {
        public void Configure(EntityTypeBuilder<InvitationSummaryModel> builder)
        {
            builder.ToTable("Invitations");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Type);
            builder.Property(p => p.Status);
            builder.Property(p => p.RecipientEmail);
            builder.Property(p => p.CreatedAt);
            builder.Property(p => p.ExpiresAt);

            builder.Property<Guid>("AccountId").HasColumnName("AccountId");
        }
    }
}
