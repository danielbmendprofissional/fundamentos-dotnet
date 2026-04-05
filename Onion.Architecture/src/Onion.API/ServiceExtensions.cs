using Microsoft.OpenApi.Models;

namespace Onion.API
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Calendar API", Version = "v1" });
            });

            return services;
        }
    }
}