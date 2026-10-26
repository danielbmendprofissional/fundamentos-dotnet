using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Task, async/await, cancelamento.</summary>
[Route("api/concorrencia/async-await")]
[Tags("03.02 - Async/Await e Tasks")]
public class AsyncAwaitController : EstudoControllerBase
{
    /// <summary>async/await com Task.Delay (não bloqueia a thread).</summary>
    [HttpGet("basico")]
    public IActionResult Basico() => NaoImplementado();

    /// <summary>Executar tarefas em paralelo e aguardar todas.</summary>
    [HttpGet("when-all")]
    public IActionResult WhenAll() => NaoImplementado();

    /// <summary>Aguardar a primeira que terminar.</summary>
    [HttpGet("when-any")]
    public IActionResult WhenAny() => NaoImplementado();

    /// <summary>CancellationToken.</summary>
    [HttpGet("cancelamento")]
    public IActionResult Cancelamento() => NaoImplementado();

    /// <summary>Por que .Result e .Wait() são perigosos.</summary>
    [HttpGet("deadlock-sync-over-async")]
    public IActionResult DeadlockSyncOverAsync() => NaoImplementado();
}
