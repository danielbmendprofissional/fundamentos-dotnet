using Galo.Estudos.Jobs;
using Galo.Estudos.Persistence;
using Galo.Infrastructure.Persistence.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Galo.Estudos.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>Registra os laboratórios de estudo (usa a conexão de runtime ConnectionStrings:Postgres).</summary>
        public static IServiceCollection AddEstudos(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Chave ConnectionStrings:Postgres não encontrada (user-secrets).");

            // Único lugar com LAZY LOADING: UseLazyLoadingProxies faz o EF gerar subclasses (proxies) das entidades
            // que interceptam o acesso às navegações "virtual" e disparam o SELECT sob demanda.
            services.AddDbContext<EstudosDbContext>(opt => opt
                .UseLazyLoadingProxies()
                .AddInterceptors(ContadorSqlInterceptor.Instancia)
                .UseNpgsql(connectionString));

            services.AddScoped<EfCoreLab>();
            services.AddScoped<ParalelismoLab>();
            services.AddScoped<MemoriaLab>();
            services.AddScoped<RuntimeLab>();
            services.AddScoped<ProcessamentoLab>();
            services.AddScoped<CacheLab>();
            services.AddScoped<ConcorrenciaLab>();

            services.AddSingleton<FilaDeJobs>();
            services.AddHostedService<JobWorker>();

            return services;
        }
    }
}
