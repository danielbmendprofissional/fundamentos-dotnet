using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Appointments.Entities;
using Onion.Infrastructure.Persistence.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.Mappings
{
    internal sealed class AppointmentMap: EntityMap<Appointment>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.Property(p => p.AccountId).IsRequired();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).HasMaxLength(255);
            builder.Property(p => p.Image).HasMaxLength(500);
            builder.Property(p => p.AppointmentType).HasConversion<int>().IsRequired();
            
            builder.Property(p => p.StartDate)
                .HasColumnType("date")
                .IsRequired();
            
            builder.Property(p => p.EndDate)
                .HasColumnType("date")
                .IsRequired();

            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(ad => ad.Street).HasColumnName("Street").IsRequired().HasMaxLength(150);
                a.Property(ad => ad.Number).HasColumnName("Number").IsRequired().HasMaxLength(20);
                a.Property(ad => ad.City).HasColumnName("City").IsRequired().HasMaxLength(100);
                a.Property(ad => ad.State).HasColumnName("State").IsRequired().HasMaxLength(50);
                a.Property(ad => ad.Country).HasColumnName("Country").IsRequired().HasMaxLength(50);
                a.Property(ad => ad.Complement).HasColumnName("Complement").HasMaxLength(150);


                a.OwnsOne(ad => ad.PostalCode, pc =>
                {
                    pc.Property(p => p.Value)
                      .HasColumnName("PostalCode")
                      .HasMaxLength(8)
                      .IsFixedLength()
                      .IsRequired();
                });
            });

            builder.OwnsMany(p => p.Dates, d =>
            {
                d.ToTable("AppointmentDates");
                d.HasKey(x => x.Id);
                d.WithOwner().HasForeignKey("AppointmentId");
                d.Property(x => x.Name).IsRequired().HasMaxLength(100);
                
                d.Property(x => x.StartDate)
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                d.Property(x => x.EndDate)
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            });

            builder.OwnsMany(p => p.Participants, pt =>
            {
                pt.ToTable("AppointmentParticipants");
                pt.HasKey(x => x.Id);
                pt.WithOwner().HasForeignKey("AppointmentId");
                pt.Property(x => x.Name).IsRequired().HasMaxLength(100);
                pt.Property(x => x.Email).IsRequired().HasMaxLength(150);
                pt.Property(x => x.Situation).HasConversion<int>().IsRequired();
                pt.Property(x => x.PartnerId).IsRequired();
            });

            builder.Navigation(p => p.Dates).Metadata.SetField("_dates");
            builder.Navigation(p => p.Participants).Metadata.SetField("_participants");
        }
    }
}
