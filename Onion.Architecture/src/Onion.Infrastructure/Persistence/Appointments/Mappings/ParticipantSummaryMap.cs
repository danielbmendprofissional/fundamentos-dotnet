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
    internal sealed class ParticipantSummaryMap : IEntityTypeConfiguration<ParticipantSummaryModel>
    {
        public void Configure(EntityTypeBuilder<ParticipantSummaryModel> builder)
        {
            builder.ToTable("AppointmentParticipants");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name);

            builder.Property<Guid>("AppointmentId").HasColumnName("AppointmentId");
        }
    }
}
