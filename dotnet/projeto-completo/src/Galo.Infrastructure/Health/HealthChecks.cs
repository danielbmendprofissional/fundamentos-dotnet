using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Galo.Infrastructure.Health
{
    // HEALTH CHECKS: o /health devolve só "Healthy/Unhealthy" (nada de connection string, host ou senha na resposta).
    // Uma verificação por dependência => quem monitora enxerga QUAL dependência caiu.

    public sealed class PostgresHealthCheck : IHealthCheck
    {
        private readonly Persistence.GaloQueryDbContext _db;
        public PostgresHealthCheck(Persistence.GaloQueryDbContext db) => _db = db;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _db.Database.CanConnectAsync(cancellationToken)
                    ? HealthCheckResult.Healthy("PostgreSQL respondendo")
                    : HealthCheckResult.Unhealthy("PostgreSQL não respondeu");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao conectar no PostgreSQL", ex);
            }
        }
    }

    public sealed class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisHealthCheck(IConnectionMultiplexer redis) => _redis = redis;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // PING é permitido pela ACL; não usamos INFO/CONFIG/KEYS (proibidos).
                var latencia = await _redis.GetDatabase().PingAsync();
                return HealthCheckResult.Healthy($"Redis respondendo em {latencia.TotalMilliseconds:F1} ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao conectar no Redis", ex);
            }
        }
    }

    public sealed class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly Messaging.RabbitMqConnection _connection;
        public RabbitMqHealthCheck(Messaging.RabbitMqConnection connection) => _connection = connection;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!_connection.Options.Habilitado) return HealthCheckResult.Healthy("Mensageria desabilitada por configuração");

            try
            {
                var conexao = await _connection.ObterAsync(cancellationToken);
                return conexao.IsOpen
                    ? HealthCheckResult.Healthy("RabbitMQ conectado")
                    : HealthCheckResult.Unhealthy("Conexão com o RabbitMQ fechada");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao conectar no RabbitMQ", ex);
            }
        }
    }
}
