using Galo.Application.Common.Interfaces.Caching;
using Galo.Application.Common.Interfaces.IntegrationEvents;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Application.Competicoes.Interfaces;
using Galo.Application.Elenco.Interfaces;
using Galo.Application.Ingressos.Interfaces;
using Galo.Application.Torcida.Interfaces;
using Galo.Domain.Competicoes.Interfaces;
using Galo.Domain.Elenco.Interfaces;
using Galo.Domain.Ingressos.Interfaces;
using Galo.Domain.Torcida.Interfaces;
using Galo.Infrastructure.Caching;
using Galo.Infrastructure.Health;
using Galo.Infrastructure.Messaging;
using Galo.Infrastructure.Persistence;
using Galo.Infrastructure.Persistence.Diagnostics;
using Galo.Infrastructure.Seed;
using Galo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Galo.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Chaves lidas (todas vêm dos user-secrets em Development):
        ///   ConnectionStrings:Postgres, ConnectionStrings:Redis, Redis:KeyPrefix, RabbitMQ:*.
        /// </summary>
        public static IServiceCollection ConfigureInfrastructureApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgres(configuration);
            services.AddRedisCache(configuration);
            services.AddRabbitMq(configuration);
            services.AddRepositories();

            // Handlers de eventos de integração que vivem na Infrastructure (ex.: envio de e-mail)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly));

            return services;
        }

        private static void AddPostgres(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("Chave ConnectionStrings:Postgres não encontrada (user-secrets).");

            // Dois DbContexts, mesma conexão (usuário "app", sem DDL):
            //   GaloDbContext      => escrita (agregados, com tracking e eventos)
            //   GaloQueryDbContext => leitura (query models, NoTracking)
            services.AddDbContext<GaloDbContext>(opt => opt.AddInterceptors(ContadorSqlInterceptor.Instancia).UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(maxRetryCount: 3);   // retenta falhas transitórias de rede (não conflitos de negócio)
            }));

            // POOL de DbContexts de leitura: reutiliza instâncias (reset entre usos) em vez de criar uma nova a cada request.
            // A factory também serve ao laboratório de paralelismo (DbContext NÃO é thread-safe: 1 por tarefa paralela).
            services.AddPooledDbContextFactory<GaloQueryDbContext>(opt => opt.AddInterceptors(ContadorSqlInterceptor.Instancia).UseNpgsql(connectionString));
            services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<GaloQueryDbContext>>().CreateDbContext());
        }

        private static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Secao));

            var redisConnection = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Chave ConnectionStrings:Redis não encontrada (user-secrets).");
            var prefixo = configuration[$"{RedisOptions.Secao}:KeyPrefix"] ?? "estudos:";

            // UMA conexão multiplexada para todo o processo (StackExchange.Redis é thread-safe e feito para ser singleton).
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var cfg = ConfigurationOptions.Parse(redisConnection);
                cfg.AbortOnConnectFail = false;       // não derruba a API na subida se o Redis estiver fora
                cfg.ClientName = "galo-api";
                return ConnectionMultiplexer.Connect(cfg);
            });

            // IDistributedCache sobre o MESMO multiplexador. InstanceName = prefixo exigido pela ACL do Redis.
            services.AddStackExchangeRedisCache(o => o.InstanceName = prefixo);
            services.AddOptions<RedisCacheOptions>().Configure<IConnectionMultiplexer>(
                (o, multiplexador) => o.ConnectionMultiplexerFactory = () => Task.FromResult(multiplexador));

            services.AddMemoryCache();
            services.AddSingleton<ICacheStore, TwoLevelCacheStore>();
            services.AddHostedService<CacheInvalidationListener>();
        }

        private static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Secao));

            services.AddSingleton<RabbitMqConnection>();
            services.AddSingleton<IntegrationEventTypeRegistry>();
            services.AddSingleton<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();
            services.AddHostedService<RabbitMqConsumerService>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            // Scoped = 1 instância por requisição HTTP (ou por mensagem consumida), compartilhando o MESMO DbContext.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DadosIniciaisSeeder>();

            services.AddScoped<IJogadorRepository, JogadorRepository>();
            services.AddScoped<IClubeRepository, ClubeRepository>();
            services.AddScoped<IEstadioRepository, EstadioRepository>();
            services.AddScoped<ICampeonatoRepository, CampeonatoRepository>();
            services.AddScoped<IPartidaRepository, PartidaRepository>();
            services.AddScoped<ITorcedorRepository, TorcedorRepository>();
            services.AddScoped<ICompraIngressoRepository, CompraIngressoRepository>();

            services.AddScoped<IJogadorQueryRepository, JogadorQueryRepository>();
            services.AddScoped<IPartidaQueryRepository, PartidaQueryRepository>();
            services.AddScoped<ICatalogoQueryRepository, CatalogoQueryRepository>();
            services.AddScoped<ITorcedorQueryRepository, TorcedorQueryRepository>();
            services.AddScoped<ICompraQueryRepository, CompraQueryRepository>();
        }

        public static IHealthChecksBuilder AddGaloHealthChecks(this IServiceCollection services)
            => services.AddHealthChecks()
                .AddCheck<PostgresHealthCheck>("postgres", tags: new[] { "db" })
                .AddCheck<RedisHealthCheck>("redis", tags: new[] { "cache" })
                .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: new[] { "mensageria" });
    }
}
