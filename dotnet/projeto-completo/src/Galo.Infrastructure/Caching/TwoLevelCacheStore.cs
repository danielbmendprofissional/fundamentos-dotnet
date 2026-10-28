using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Galo.Application.Common.Interfaces.Caching;
using Galo.Application.Common.Telemetry;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Galo.Infrastructure.Caching
{
    /// <summary>
    /// CACHE EM 2 NÍVEIS + CACHE-ASIDE (o "padrão" que mais cai em entrevista e mais dá problema em produção).
    ///
    ///  L1  IMemoryCache      : dentro do processo. Nanossegundos, sem rede, mas cada instância tem a sua cópia.
    ///  L2  IDistributedCache : Redis. Microssegundos/milissegundos, compartilhado entre instâncias.
    ///  Banco                 : só no "miss" dos dois.
    ///
    /// Armadilhas tratadas aqui (cada uma é uma pergunta de entrevista):
    ///  1. CACHE STAMPEDE (dogpile): 1000 requests no mesmo miss => 1000 idas ao banco. Solução: SemaphoreSlim POR CHAVE
    ///     + double-check, para só UM executar o factory e os outros esperarem o resultado.
    ///  2. INVALIDAÇÃO por grupo SEM "KEYS/SCAN" (proibidos nesta infra): cada chave nova é registrada em um SET
    ///     "{prefixo}cache-group:{grupo}"; invalidar = ler o SET, apagar cada chave e apagar o SET.
    ///  3. L1 DESATUALIZADO em outras instâncias: a invalidação é publicada em um canal pub/sub; cada instância
    ///     (ver CacheInvalidationListener) evicta o L1 local. Como rede falha, o L1 tem TTL curto (TtlMaximoL1).
    ///  4. Serialização: System.Text.Json. Tipos precisam ser serializáveis (records de DTO, ok).
    /// </summary>
    public sealed class TwoLevelCacheStore : ICacheStore
    {
        internal const string CanalInvalidacao = "cache-invalidate";

        private readonly IMemoryCache _l1;
        private readonly IDistributedCache _l2;
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisOptions _options;
        private readonly ILogger<TwoLevelCacheStore> _logger;

        // um semáforo por chave (stampede protection)
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _travas = new();
        // índice LOCAL grupo -> chaves no L1 (para poder evictar o grupo sem varrer o IMemoryCache, que não permite listar)
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _indiceL1 = new();

        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        public TwoLevelCacheStore(IMemoryCache l1, IDistributedCache l2, IConnectionMultiplexer redis,
            IOptions<RedisOptions> options, ILogger<TwoLevelCacheStore> logger)
        {
            _l1 = l1;
            _l2 = l2;
            _redis = redis;
            _options = options.Value;
            _logger = logger;
        }

        private string ChaveIndice(string grupo) => $"{_options.KeyPrefix}cache-group:{grupo}";
        internal string Canal => $"{_options.KeyPrefix}{CanalInvalidacao}";

        public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan ttl,
            string? group = null, CancellationToken cancellationToken = default)
        {
            // ---- L1 ----
            if (_l1.TryGetValue(key, out T? valorL1) && valorL1 is not null)
            {
                GaloTelemetry.CacheHits.Add(1, new KeyValuePair<string, object?>("nivel", "L1"));
                return valorL1;
            }

            // ---- L2 ----
            var valorL2 = await LerL2Async<T>(key, cancellationToken);
            if (valorL2 is not null)
            {
                GaloTelemetry.CacheHits.Add(1, new KeyValuePair<string, object?>("nivel", "L2"));
                GravarL1(key, valorL2, ttl, group);
                return valorL2;
            }

            // ---- MISS: só UM por chave executa o factory ----
            var trava = _travas.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await trava.WaitAsync(cancellationToken);
            try
            {
                // double-check: quem esperou a trava pode encontrar o valor já preenchido por outro
                if (_l1.TryGetValue(key, out valorL1) && valorL1 is not null)
                {
                    GaloTelemetry.CacheHits.Add(1, new KeyValuePair<string, object?>("nivel", "L1"));
                    return valorL1;
                }

                GaloTelemetry.CacheMisses.Add(1);
                var valor = await factory(cancellationToken);

                if (valor is not null)
                {
                    await GravarL2Async(key, valor, ttl, group, cancellationToken);
                    GravarL1(key, valor, ttl, group);
                }

                return valor;
            }
            finally
            {
                trava.Release();
                // Remover a trava evita vazamento de memória (milhares de chaves). Uma corrida rara aqui só causa
                // uma execução extra do factory - aceitável e inofensiva.
                _travas.TryRemove(key, out _);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _l1.Remove(key);
            try { await _l2.RemoveAsync(key, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning("Falha ao remover {Chave} do Redis: {Msg}", key, ex.Message); }
        }

        public async Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default)
        {
            // L1 local imediatamente
            EvictarGrupoLocal(group);

            try
            {
                var db = _redis.GetDatabase();
                var indice = ChaveIndice(group);

                var chaves = await db.SetMembersAsync(indice);
                foreach (var chave in chaves)
                    await _l2.RemoveAsync(chave.ToString(), cancellationToken);

                await db.KeyDeleteAsync(indice);

                // avisa as OUTRAS instâncias (e esta, inofensivo) para evictarem o L1
                await _redis.GetSubscriber().PublishAsync(RedisChannel.Literal(Canal), group);

                _logger.LogInformation("Grupo de cache {Grupo} invalidado ({Qtd} chaves)", group, chaves.Length);
            }
            catch (Exception ex)
            {
                // Cache é otimização: se o Redis falhar, a aplicação continua (apenas dados possivelmente defasados até o TTL).
                _logger.LogWarning("Falha ao invalidar grupo {Grupo} no Redis: {Msg}", group, ex.Message);
            }
        }

        /// <summary>Chamado pelo listener do pub/sub (e localmente): apaga do L1 as chaves do grupo.</summary>
        internal void EvictarGrupoLocal(string group)
        {
            if (_indiceL1.TryRemove(group, out var chaves))
                foreach (var chave in chaves.Keys) _l1.Remove(chave);
        }

        // ---------------- helpers ----------------

        private void GravarL1<T>(string key, T valor, TimeSpan ttl, string? group)
        {
            var ttlL1 = ttl < _options.TtlMaximoL1 ? ttl : _options.TtlMaximoL1;
            _l1.Set(key, valor, ttlL1);

            if (group is not null)
                _indiceL1.GetOrAdd(group, _ => new ConcurrentDictionary<string, byte>())[key] = 0;
        }

        private async Task<T?> LerL2Async<T>(string key, CancellationToken ct)
        {
            try
            {
                var bytes = await _l2.GetAsync(key, ct);
                return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, Json);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Redis fora do ar NÃO pode derrubar a consulta: degrada para "miss" e vai ao banco.
                _logger.LogWarning("L2 indisponível ao ler {Chave}: {Msg}", key, ex.Message);
                return default;
            }
        }

        private async Task GravarL2Async<T>(string key, T valor, TimeSpan ttl, string? group, CancellationToken ct)
        {
            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(valor, Json);
                await _l2.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl }, ct);

                if (group is not null)
                {
                    // A chave real no Redis tem o InstanceName (prefixo) na frente; o SET guarda a chave LÓGICA,
                    // que é o que IDistributedCache.RemoveAsync espera.
                    var db = _redis.GetDatabase();
                    await db.SetAddAsync(ChaveIndice(group), key);
                    await db.KeyExpireAsync(ChaveIndice(group), ttl + TimeSpan.FromMinutes(1)); // índice não vive para sempre
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning("L2 indisponível ao gravar {Chave}: {Msg}", key, ex.Message);
            }
        }
    }
}
