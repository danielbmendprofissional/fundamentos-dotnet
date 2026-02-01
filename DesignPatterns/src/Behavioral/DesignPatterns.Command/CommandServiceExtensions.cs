using DesignPatterns.Command.Bad;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command
{
    public static class CommandServiceExtensions
    {
        public static IServiceCollection AddCommandServices(this IServiceCollection services)
        {
            // OBRIGATORIEDADE: Registro do MediatR. Ele escaneia o assembly em busca de Handlers.
            // Padrão Microsoft para evitar o registro manual de cada comando.
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CommandServiceExtensions).Assembly));

            // Registro do Receiver
            services.AddSingleton<EstoqueService>();

            return services;
        }
    }
}
