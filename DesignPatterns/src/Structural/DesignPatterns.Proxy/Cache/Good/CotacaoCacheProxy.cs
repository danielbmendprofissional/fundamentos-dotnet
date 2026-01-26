using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Cache.Good
{
    public class CotacaoCacheProxy : ICotacaoService
    {
        private readonly ICotacaoService _realService;
        
        // The RAM cache service from Microsoft
        // the _cacheKey is used to identify the cached item, example "pessoa_id_123" 

        private readonly IMemoryCache _cache;
        private readonly string _cacheKey = "PrecoDolar";

        public CotacaoCacheProxy(ICotacaoService realService, IMemoryCache cache)
        {
            _realService = realService;
            _cache = cache;
        }

        public decimal ObterPrecoDolar()
        {
            // Trys to get the value from cache, if not found, calls the real service and stores the result in cache
            return _cache.GetOrCreate(_cacheKey, entry => {
                // Expiration time of 10 seconds
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

                Console.WriteLine("[Proxy] Cache expirado ou vazio. Consultando service real...");
                return _realService.ObterPrecoDolar();
            });
        }
    }
}
