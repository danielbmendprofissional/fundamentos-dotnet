using DesignPatterns.Observer.Good.Microsoft;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Observer
{
    public static class DependencyInjectionExtensions
    {
        // OBRIGATORIEDADE: O uso de ServiceExtensions é o padrão profissional para manter o Program limpo.
        public static IServiceCollection AddEstoqueServices(this IServiceCollection services)
        {
            // OBRIGATORIEDADE: O Subject (Estoque) DEVE ser Singleton neste cenário.
            // Se for Transient ou Scoped, os observadores assinarão instâncias diferentes e nunca receberão o evento.
            services.AddSingleton<Estoque>();

            // OBRIGATORIEDADE: Os observadores precisam ser registrados para que o DI os instancie e execute o construtor.
            services.AddSingleton<MarketingService>();
            services.AddSingleton<ComprasService>();

            return services;
        }
    }
}
