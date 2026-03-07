using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Mesaging.Interfaces;
using Valuto.AuditTrail.Mesaging.Options;

namespace Valuto.AuditTrail.Mesaging
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuditMessaging(this IServiceCollection services, Enviroments enviroment)
        {
            if (enviroment == Enviroments.Producao)
            {
                services.AddSingleton<IAuditOptions, AuditOptionsPrd>();
            }
            else
            {
                services.AddSingleton<IAuditOptions, AuditOptionsHml>();
            }

            services.AddSingleton<IAuditBusProvider, AuditBusProvider>();

            services.AddSingleton<IAuditBusProvider, AuditBusProvider>();

            return services;
        }
    }
}
