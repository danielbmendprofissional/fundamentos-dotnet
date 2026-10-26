using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>IMemoryCache: guardar resultados caros.</summary>
[Route("api/memoria/cache")]
[Tags("02.03 - Cache em memória")]
public class CacheController : EstudoControllerBase
{
    /// <summary>GetOrCreate.</summary>
    [HttpGet("obter-ou-criar")]
    public IActionResult ObterOuCriar() => NaoImplementado();

    /// <summary>Expiração absoluta x deslizante.</summary>
    [HttpGet("expiracao")]
    public IActionResult Expiracao() => NaoImplementado();

    /// <summary>Remover item do cache.</summary>
    [HttpDelete("invalidar")]
    public IActionResult Invalidar() => NaoImplementado();
}
