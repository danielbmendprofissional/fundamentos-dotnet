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
    internal sealed class ParticipantDetailsMap
        : IEntityTypeConfiguration<ParticipantDetailsModel>
    {
        public void Configure(EntityTypeBuilder<ParticipantDetailsModel> builder)
        {
            builder.ToTable("AppointmentParticipants");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name);
            builder.Property(p => p.Email);
            builder.Property(p => p.Situation);

            builder.Property<Guid>("AppointmentId").HasColumnName("AppointmentId");
        }
    }
}
