using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence
{
    internal class UserMap
        : EntityMap<User>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");

            builder.OwnsOne(c => c.Name, nome =>
            {
                nome.Property(n => n.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(150);

                nome.Property(n => n.LastName)
                    .HasColumnName("last_name")
                    .IsRequired()
                    .HasMaxLength(150);

                nome.Property(n => n.NormalizedName)
                    .HasColumnName("normalized_name")
                    .IsRequired()
                    .HasMaxLength(300);
            });

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Adress)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(e => e.Adress).IsUnique();
            });

            builder.OwnsOne(c => c.Phone, telefone =>
            {
                telefone.Property(t => t.Number)
                    .HasColumnName("phone")
                    .IsRequired()
                    .HasMaxLength(11);
            });

            builder.Property(c => c.Orientation)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
