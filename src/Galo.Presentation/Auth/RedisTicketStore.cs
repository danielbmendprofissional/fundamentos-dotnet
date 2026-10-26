using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Galo.Presentation.Auth
{
    /// <summary>
    /// SESSÃO NO SERVIDOR (padrão BFF). Por padrão o cookie de autenticação carrega o ticket INTEIRO (claims + tokens do
    /// Keycloak, criptografados) para o navegador. Aqui o cookie guarda só um ID opaco; o ticket (com access/refresh
    /// token) fica no Redis. Resultado: o navegador NUNCA recebe tokens do Keycloak, e dá para revogar sessões.
    ///
    /// A chave no Redis recebe automaticamente o prefixo "estudos:" (InstanceName do IDistributedCache).
    /// </summary>
    public sealed class RedisTicketStore : ITicketStore
    {
        private const string Prefixo = "auth-session:";
        private readonly IDistributedCache _cache;

        public RedisTicketStore(IDistributedCache cache) => _cache = cache;

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var chave = Prefixo + Guid.NewGuid().ToString("N");
            await RenewAsync(chave, ticket);
            return chave;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var opcoes = new DistributedCacheEntryOptions();
            if (ticket.Properties.ExpiresUtc is { } expira) opcoes.AbsoluteExpiration = expira;
            else opcoes.SlidingExpiration = TimeSpan.FromHours(8);

            return _cache.SetAsync(key, TicketSerializer.Default.Serialize(ticket), opcoes);
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            var bytes = await _cache.GetAsync(key);
            return bytes is null ? null : TicketSerializer.Default.Deserialize(bytes);
        }

        public Task RemoveAsync(string key) => _cache.RemoveAsync(key);
    }
}
