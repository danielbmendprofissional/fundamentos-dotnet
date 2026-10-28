using System.Reflection;
using Galo.Application.Common.Behaviors;
using Galo.Domain.Ingressos.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Galo.Application.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureApplicationApp(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                // varre este assembly atrás de IRequestHandler / INotificationHandler
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                // pipeline: roda em volta de TODOS os handlers, na ordem de registro
                cfg.AddOpenBehavior(typeof(TelemetryBehavior<,>));
            });

            // Domain services são stateless: singleton.
            services.AddSingleton<ICalculadoraDeDescontoService, CalculadoraDeDescontoService>();

            return services;
        }
    }
}
