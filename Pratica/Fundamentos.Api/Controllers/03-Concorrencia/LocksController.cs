using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Sincronização de acesso a estado compartilhado.</summary>
[Route("api/concorrencia/locks")]
[Tags("03.03 - Locks e race conditions")]
public class LocksController : EstudoControllerBase
{
    /// <summary>Contador sem proteção: observar o resultado errado.</summary>
    [HttpGet("race-condition")]
    public IActionResult RaceCondition() => NaoImplementado();

    /// <summary>Corrigir com lock.</summary>
    [HttpGet("lock")]
    public IActionResult Lock() => NaoImplementado();

    /// <summary>Incremento atômico.</summary>
    [HttpGet("interlocked")]
    public IActionResult Interlocked() => NaoImplementado();

    /// <summary>Limitar concorrência (funciona com async).</summary>
    [HttpGet("semaphore-slim")]
    public IActionResult SemaphoreSlim() => NaoImplementado();

    /// <summary>Provocar e entender um deadlock.</summary>
    [HttpGet("deadlock")]
    public IActionResult Deadlock() => NaoImplementado();
}
