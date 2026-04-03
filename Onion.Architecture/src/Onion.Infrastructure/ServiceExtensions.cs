using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Appointments.Interfaces;
using Onion.Application.Common.Interfaces;
using Onion.Application.Invitations.Interfaces;
using Onion.Application.Partners.Interfaces;
using Onion.Domain.Accounts.Interfaces;
using Onion.Domain.Appointments.Interfaces;
using Onion.Domain.Partners.Interfaces;
using Onion.Infrastructure.Persistence.Accounts.Repositories;
using Onion.Infrastructure.Persistence.Appointments.Repositories;
using Onion.Infrastructure.Persistence.Common;
using Onion.Infrastructure.Persistence.Common.Context;
using Onion.Infrastructure.Persistence.Invitations.Repositories;
using Onion.Infrastructure.Persistence.Partners.Repositories;

namespace Onion.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CalendarContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IAccountQueryRepository, AccountQueryRepository>();
            services.AddScoped<IAppointmentQueryRepository, AppointmentQueryRepository>();
            services.AddScoped<IPartnerQueryRepository, PartnerQueryRepository>();
            services.AddScoped<IInvitationQueryRepository, InvitationQueryRepository>();

            return services;
        }
    }
}