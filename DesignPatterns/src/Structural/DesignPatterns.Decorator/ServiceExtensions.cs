using DesignPatterns.Decorator._2;
using DesignPatterns.Decorator._2.Base;
using DesignPatterns.Decorator.Bad;
using DesignPatterns.Decorator.Good;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPatterns.Decorator
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDecoratorServices(this IServiceCollection services)
        {
            // --- OPTION 1: THE MANUAL/NATIVE WAY (No external libraries) ---
            // Use this if you want full control or don't want to add dependencies.

            // 1. Register the concrete implementation first (The "Core")
            services.AddScoped<RemoteWeatherService>();

            // 2. Register the interface using a factory function
            services.AddScoped<IWeatherService>(provider =>
            {
                // Get the real instance from the provider
                var realService = provider.GetRequiredService<RemoteWeatherService>();

                // Manually wrap it. Here you can pass '3' or any logic for maxRetries.
                return new RetryWeatherDecorator(realService, 3);
            });


            // --- OPTION 2: THE SCRUTOR WAY (Professional/Clean) ---
            // Use this for production projects to avoid "Factory Hell".

            /* // IMPORTANT: To use this, comment out Option 1 first!

            // 1. Register the base implementation normally
            services.AddScoped<IWeatherService, RemoteWeatherService>();

            // 2. Use Scrutor to "Wrap" it. 
            // Since RetryWeatherDecorator needs an 'int', we use a lambda to help Scrutor:
            services.Decorate<IWeatherService>((inner, provider) => 
                new RetryWeatherDecorator(inner, 3)
            );
            */

            return services;
        }

        public static IServiceCollection AddPedidoServices(this IServiceCollection services)
        {
            services.AddScoped<PedidoBase>();

            services.AddScoped<IPedido>(provider => {
                var pedidoReal = provider.GetRequiredService<PedidoBase>();
                return new BlackFridayDecorator(pedidoReal);
            });

            return services;
        }
    }
}
