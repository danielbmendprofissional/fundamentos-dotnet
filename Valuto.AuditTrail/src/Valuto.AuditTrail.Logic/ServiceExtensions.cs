using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Logic.Cache;
using Valuto.AuditTrail.Logic.Paralel;
using Valuto.AuditTrail.Logic.Providers;
using Valuto.AuditTrail.Mesaging;

namespace Valuto.AuditTrail.Logic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuditLogic(this IServiceCollection services)
        {
            services.AddAuditMessaging();

            services.AddScoped<IAuditoriaInterna, AuditoriaInterna>();

            services.TryAddScoped<IUserProvider, DefaultUserProvider>();

            return services;
        }

        public static void RegisterAuditEngine<T>(
            this IServiceCollection services) where T : class
        {
            services.AddScoped<IBusService<T>, BusService<T>>();

            services.AddSingleton<IAuditBuffer<T>, AuditBuffer<T>>();

            services.AddSingleton<IHostedService, AuditDispatcherService<T>>(sp =>
            {
                return ActivatorUtilities.CreateInstance<AuditDispatcherService<T>>(sp);
            });
        }
    }
}
