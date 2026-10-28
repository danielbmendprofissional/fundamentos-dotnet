using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Onion.Application.Accounts.Interfaces;
using Onion.Application.Accounts.Services;
using Onion.Application.Appointments.Interfaces;
using Onion.Application.Appointments.Services;
using Onion.Application.Invitations.Interfaces;
using Onion.Application.Invitations.Services;
using Onion.Application.Partners.Interfaces;
using Onion.Application.Partners.Services;
using System.Linq;
using System.Reflection;

namespace Onion.Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IPartnerService, PartnerService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
