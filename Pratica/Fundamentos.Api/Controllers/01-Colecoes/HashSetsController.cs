using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>HashSet&lt;T&gt;: conjunto sem duplicados.</summary>
[Route("api/colecoes/hashsets")]
[Tags("01.05 - HashSets")]
public class HashSetsController : EstudoControllerBase
{
    /// <summary>Remover e detectar duplicados.</summary>
    [HttpGet("duplicados")]
    public IActionResult Duplicados() => NaoImplementado();

    /// <summary>UnionWith, IntersectWith, ExceptWith.</summary>
    [HttpGet("operacoes-de-conjunto")]
    public IActionResult OperacoesDeConjunto() => NaoImplementado();

    /// <summary>Medir Contains em List x HashSet com 100 mil itens (Stopwatch).</summary>
    [HttpGet("list-vs-hashset")]
    public IActionResult ListVsHashSet() => NaoImplementado();
}
