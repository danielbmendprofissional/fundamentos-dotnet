using DesignPatterns.Strategy.Bad;
using DesignPatterns.Strategy.Good.Microsoft;
using DesignPatterns.Strategy.Good.Microsoft.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good
{
    public static class FreteServiceExtensions
    {
        public static IServiceCollection AddMicrosoftStrategyServices(this IServiceCollection services)
        {
            // OBRIGATORIEDADE: No .NET 8+, usamos chaves para registrar implementações distintas.
            // Isso associa um "token" (neste caso, o Enum ou String) diretamente à classe.
            services.AddKeyedSingleton<IFreteStrategy, FreteNormalStrategy>(TipoFrete.Normal);
            services.AddKeyedSingleton<IFreteStrategy, FreteExpressoStrategy>(TipoFrete.Expresso);
            services.AddKeyedSingleton<IFreteStrategy, FreteInternacionalStrategy>(TipoFrete.Internacional);

            return services;

            return services;
        }
    }
}
