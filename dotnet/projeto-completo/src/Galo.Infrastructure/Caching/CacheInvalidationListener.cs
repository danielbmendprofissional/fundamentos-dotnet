using Galo.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Galo.Infrastructure.Caching
{
    /// <summary>
    /// Assina o canal pub/sub do Redis e evicta o L1 local quando QUALQUER instância invalida um grupo.
    /// Sem isto, com 2+ instâncias da API, a instância B serviria dado velho do seu L1 após a A invalidar.
    /// </summary>
    public sealed class CacheInvalidationListener : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ICacheStore _cache;
        private readonly ILogger<CacheInvalidationListener> _logger;

        public CacheInvalidationListener(IConnectionMultiplexer redis, ICacheStore cache, ILogger<CacheInvalidationListener> logger)
        {
            _redis = redis;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_cache is not TwoLevelCacheStore store) return;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var subscriber = _redis.GetSubscriber();
                    await subscriber.SubscribeAsync(RedisChannel.Literal(store.Canal), (_, mensagem) =>
                    {
                        var grupo = mensagem.ToString();
                        if (!string.IsNullOrWhiteSpace(grupo)) store.EvictarGrupoLocal(grupo);
                    });

                    _logger.LogInformation("Escutando invalidações de cache em {Canal}", store.Canal);
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    _logger.LogWarning("Pub/sub do Redis indisponível ({Msg}); nova tentativa em 10 s", ex.Message);
                    try { await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); } catch (OperationCanceledException) { }
                }
            }
        }
    }
}
