using DesignPatterns.Composite.FirstExample;
using DesignPatterns.Composite.SecondExample;
using DesignPatterns.Composite.SecondExample.Factory;
using DesignPatterns.Composite.ThirdExample.Factory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogisticsModule(this IServiceCollection services)
        {
            // In .NET, it's common to use factories to create complex objects
            services.AddScoped<ILogisticsFactory, LogisticsFactory>();

            // Example of registering a default logistics item using the factory
            services.AddScoped<ILogisticsItem>(provider =>
            {
                var factory = provider.GetRequiredService<ILogisticsFactory>();
                return factory.CreateDefaultInboundShipment();
            });

            return services;
        }

        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            // Registering product-related services
            // Transient because we might want different instances each time

            services.AddTransient<ProductLeaf>();
            services.AddTransient<ProductKitComposite>();

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddScoped<IPermissionFactory, PermissionFactory>();

            return services;
        }
    }
}
