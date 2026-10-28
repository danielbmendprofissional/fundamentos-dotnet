using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Fundamentos.Api.Controllers;

/// <summary>IMemoryCache: guardar resultados caros.</summary>
[Route("api/memoria/cache")]
[Tags("02.02 - Cache em memória")]
public class CacheController(IMemoryCache cache) : EstudoControllerBase
{
    /// <summary>GetOrCreate.</summary>
    [HttpGet("obter-ou-criar")]
    public async Task<IActionResult> ObterOuCriar()
    {
        var sw = Stopwatch.StartNew();
        bool veioDoCache = true;

        var tabela = await cache.GetOrCreateAsync("classificacao", async entry =>
        {
            veioDoCache = false;                                        // só entra aqui no MISS
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            await Task.Delay(1000);                                     // simula uma consulta cara
            return new[] { "Atlético", "Palmeiras", "Flamengo" };
        });

        return Ok(new { veioDoCache, tempoMs = sw.ElapsedMilliseconds, tabela });
    }

    /// <summary>Expiração absoluta x deslizante.</summary>
    [HttpGet("expiracao")]
    public async Task<IActionResult> Expiracao()
    {
        cache.Set("absoluta", "A", new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2)   // vence 2 s depois de criado
        });
        cache.Set("deslizante", "D", new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(2)                 // vence 2 s depois do ÚLTIMO acesso
        });

        await Task.Delay(1500);
        bool absolutaAos1500 = cache.TryGetValue("absoluta", out _);
        bool deslizanteAos1500 = cache.TryGetValue("deslizante", out _); // este acesso renova o prazo

        await Task.Delay(1000);                                         // total: 2500 ms
        bool absolutaAos2500 = cache.TryGetValue("absoluta", out _);
        bool deslizanteAos2500 = cache.TryGetValue("deslizante", out _);

        return Ok(new { absolutaAos1500, deslizanteAos1500, absolutaAos2500, deslizanteAos2500 });
    }

    /// <summary>Remover item do cache.</summary>
    [HttpDelete("invalidar")]
    public IActionResult Invalidar([FromQuery] string chave = "classificacao")
    {
        bool existiaAntes = cache.TryGetValue(chave, out _);
        cache.Remove(chave);
        bool existeDepois = cache.TryGetValue(chave, out _);

        return Ok(new { chave, existiaAntes, existeDepois });
    }
}
