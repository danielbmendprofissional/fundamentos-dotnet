namespace Galo.Application.Common.Interfaces.Caching
{
    /// <summary>
    /// Cache em 2 níveis (L1 = memória do processo, L2 = Redis) com padrão cache-aside e invalidação por GRUPO.
    /// Detalhes e armadilhas (stampede, TTL, invalidação distribuída) em Galo.Infrastructure/Caching.
    /// </summary>
    public interface ICacheStore
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan ttl,
            string? group = null,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>Invalida todas as chaves de um grupo (ex.: "classificacao") sem usar KEYS/SCAN (proibidos no Redis desta infra).</summary>
        Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default);
    }
}
