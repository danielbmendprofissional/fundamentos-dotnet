using DesignPatterns.Proxy.Remote.Good;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPatterns.Proxy.Remote
{
    public static class WeatherServiceExtensions
    {
        public static IServiceCollection AddWeatherAcl(this IServiceCollection services)
        {
            // We register the HttpClient specifically for our Proxy
            services.AddHttpClient<IWeatherService, WeatherRemoteProxy>(client =>
            {
                client.BaseAddress = new Uri("https://api.external-provider.com/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
