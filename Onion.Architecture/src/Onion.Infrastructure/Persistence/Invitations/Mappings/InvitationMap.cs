using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Invitations.Entities;
using Onion.Infrastructure.Persistence.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Invitations.Mappings
{
    internal sealed class InvitationMap : EntityMap<Invitation>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Invitation> builder)
        {
            builder.ToTable("Invitations");

            builder.Property(p => p.AccountId).IsRequired();
            builder.Property(p => p.RecipientEmail).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Type).HasConversion<int>().IsRequired();
            builder.Property(p => p.Status).HasConversion<int>().IsRequired();
            builder.Property(p => p.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
            builder.Property(p => p.ExpiresAt).HasColumnType("timestamp with time zone");
            builder.Property(p => p.RespondedAt).HasColumnType("timestamp with time zone");
            builder.Property(p => p.Note).HasMaxLength(500);
            builder.Property(p => p.TargetId);
        }
    }
}
