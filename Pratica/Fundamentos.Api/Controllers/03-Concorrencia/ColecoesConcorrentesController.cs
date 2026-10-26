using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>System.Collections.Concurrent e Channels.</summary>
[Route("api/concorrencia/colecoes-concorrentes")]
[Tags("03.04 - Coleções concorrentes")]
public class ColecoesConcorrentesController : EstudoControllerBase
{
    /// <summary>List&lt;T&gt; acessada por várias threads: o que quebra.</summary>
    [HttpGet("list-sem-protecao")]
    public IActionResult ListSemProtecao() => NaoImplementado();

    /// <summary>AddOrUpdate e GetOrAdd.</summary>
    [HttpGet("concurrent-dictionary")]
    public IActionResult ConcurrentDictionary() => NaoImplementado();

    /// <summary>Fila segura entre threads.</summary>
    [HttpGet("concurrent-queue")]
    public IActionResult ConcurrentQueue() => NaoImplementado();

    /// <summary>Channel&lt;T&gt; ou BlockingCollection&lt;T&gt;.</summary>
    [HttpGet("produtor-consumidor")]
    public IActionResult ProdutorConsumidor() => NaoImplementado();
}
