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
    internal sealed class AppointmentSummaryMap
        : IEntityTypeConfiguration<AppointmentSummaryModel>
    {
        public void Configure(EntityTypeBuilder<AppointmentSummaryModel> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name);
            builder.Property(p => p.AppointmentType);
            builder.Property(p => p.StartDate);
            builder.Property(p => p.EndDate);

            builder.Property<Guid>("AccountId").HasColumnName("AccountId");
        }
    }
}
