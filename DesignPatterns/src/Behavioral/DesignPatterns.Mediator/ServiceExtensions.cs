using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMicrosoft(this IServiceCollection services)
        {
            // OBRIGATORIEDADE: O MediatR busca automaticamente todos os Handlers no assembly.
            return services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly));
        }
    }
}
