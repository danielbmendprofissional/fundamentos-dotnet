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
    internal sealed class AddressDetailsMap
        : IEntityTypeConfiguration<AddressDetailsModel>
    {
        public void Configure(EntityTypeBuilder<AddressDetailsModel> builder)
        {
            builder.ToTable("Appointments");
            builder.HasKey(p => p.AppointmentId);

            builder.Property(p => p.Street);
            builder.Property(p => p.Number);
            builder.Property(p => p.Complement);
            builder.Property(p => p.City);
            builder.Property(p => p.State);
            builder.Property(p => p.PostalCode);
            builder.Property(p => p.Country);

        }
    }
}
