using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Mesaging.Interfaces;
using Valuto.AuditTrail.Mesaging.Options;

namespace Valuto.AuditTrail.Mesaging
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuditMessaging(this IServiceCollection services)
        {
            var options = new AuditOptions();
            services.AddSingleton(options);

            services.AddSingleton<IAuditBusProvider, AuditBusProvider>();

            return services;
        }
    }
}
