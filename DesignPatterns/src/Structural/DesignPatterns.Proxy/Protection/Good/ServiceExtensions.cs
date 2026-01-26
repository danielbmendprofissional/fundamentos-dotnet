using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Protection.Good
{
    using DesignPatterns.Proxy.Protection.Bad;
    using Microsoft.Extensions.DependencyInjection;

    //EXAMPLE FOR MORE PROTECTION WHEN USE PROXY PATTERN IN APIs
    public static class ServiceExtensions
    {
        public static IServiceCollection AddProntuarioServices(this IServiceCollection services)
        {
            // 1. Register the Repository (use interfaces...)
            services.AddScoped<ProntuarioRepository>();

            // 2. Register Serivce without interface, just the real implementation
            // because proxies need to know the real service to delegate calls.
            services.AddScoped<ProntuarioServiceReal>();

            // 3. Register the Proxy as the implementation of the Service Interface
            services.AddScoped<IProntuarioService, ProntuarioProxy>();

            return services;
        }
    }
}
