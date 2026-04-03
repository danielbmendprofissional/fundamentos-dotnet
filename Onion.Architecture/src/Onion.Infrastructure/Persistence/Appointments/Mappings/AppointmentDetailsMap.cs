using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Infrastructure.Persistence.Appointments.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.Mappings
{
    internal sealed class AppointmentDetailsMap
        : IEntityTypeConfiguration<AppointmentDetailsModel>
    {
        public void Configure(EntityTypeBuilder<AppointmentDetailsModel> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name);
            builder.Property(p => p.Description);
            builder.Property(p => p.Image);
            builder.Property(p => p.AppointmentType);
            builder.Property(p => p.StartDate);
            builder.Property(p => p.EndDate);

            builder.Property(p => p.Street).HasColumnName("Street");
            builder.Property(p => p.Number).HasColumnName("Number");

            builder.HasMany(p => p.Dates)
                .WithOne()
                .HasForeignKey("AppointmentId");

            builder.HasMany(p => p.Participants)
                .WithOne()
                .HasForeignKey("AppointmentId");

            builder.Property<Guid>("AccountId").HasColumnName("AccountId");
        }
    }
}
