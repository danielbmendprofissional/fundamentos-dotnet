using Microsoft.EntityFrameworkCore;
using Onion.Application.Appointments.DTOs.Responses;
using Onion.Infrastructure.Persistence.Accounts.Mappings;
using Onion.Infrastructure.Persistence.Accounts.QueryModels;
using Onion.Infrastructure.Persistence.Appointments.Mappings;
using Onion.Infrastructure.Persistence.Appointments.QueryModels;
using Onion.Infrastructure.Persistence.Invitations.Mappings;
using Onion.Infrastructure.Persistence.Invitations.QueryModels;
using Onion.Infrastructure.Persistence.Partners.Mappings;
using Onion.Infrastructure.Persistence.Partners.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Common.Context
{
    public sealed class CalendarQueryContext : DbContext
    {
        public DbSet<AccountDetailsModel> AccountDetails => Set<AccountDetailsModel>();
        public DbSet<AccountSummaryModel> AccountSummaries => Set<AccountSummaryModel>();

        public DbSet<AppointmentDetailsModel> AppointmentDetails => Set<AppointmentDetailsModel>();
        public DbSet<AppointmentSummaryModel> AppointmentSummaries => Set<AppointmentSummaryModel>();
        public DbSet<AddressDetailsModel> AddressDetails => Set<AddressDetailsModel>();
        public DbSet<AppointmentDateDetailsModel> AppointmentDateDetails => Set<AppointmentDateDetailsModel>();
        public DbSet<AppointmentDateSummaryModel> AppointmentDateSummaries => Set<AppointmentDateSummaryModel>();
        public DbSet<ParticipantDetailsModel> ParticipantDetails => Set<ParticipantDetailsModel>();
        public DbSet<ParticipantSummaryModel> ParticipantSummaries => Set<ParticipantSummaryModel>();

        public DbSet<InvitationDetailsModel> InvitationDetails => Set<InvitationDetailsModel>();
        public DbSet<InvitationSummaryModel> InvitationSummaries => Set<InvitationSummaryModel>();

        public DbSet<PartnerDetailsModel> PartnerDetails => Set<PartnerDetailsModel>();
        public DbSet<PartnerSummaryModel> PartnerSummaries => Set<PartnerSummaryModel>();

        public CalendarQueryContext(DbContextOptions<CalendarQueryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountDetailsMap());
            modelBuilder.ApplyConfiguration(new AccountSummaryMap());
        
            modelBuilder.ApplyConfiguration(new AppointmentDetailsMap());
            modelBuilder.ApplyConfiguration(new AppointmentSummaryMap());
            modelBuilder.ApplyConfiguration(new AddressDetailsMap());
            modelBuilder.ApplyConfiguration(new AppointmentDateDetailsMap());
            modelBuilder.ApplyConfiguration(new AppointmentDateSummaryMap());
            modelBuilder.ApplyConfiguration(new ParticipantDetailsMap());
            modelBuilder.ApplyConfiguration(new ParticipantSummaryMap());

            modelBuilder.ApplyConfiguration(new InvitationDetailsMap());
            modelBuilder.ApplyConfiguration(new InvitationSummaryMap());

            modelBuilder.ApplyConfiguration(new PartnerDetailsMap());
            modelBuilder.ApplyConfiguration(new PartnerSummaryMap());
        }
    }
}
