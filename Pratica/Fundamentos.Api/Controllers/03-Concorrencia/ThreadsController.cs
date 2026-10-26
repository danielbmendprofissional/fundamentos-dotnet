using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Thread e ThreadPool.</summary>
[Route("api/concorrencia/threads")]
[Tags("03.01 - Threads")]
public class ThreadsController : EstudoControllerBase
{
    /// <summary>Criar e iniciar uma Thread.</summary>
    [HttpGet("criar")]
    public IActionResult Criar() => NaoImplementado();

    /// <summary>Esperar o término de uma thread.</summary>
    [HttpGet("join")]
    public IActionResult Join() => NaoImplementado();

    /// <summary>ThreadPool.QueueUserWorkItem x new Thread.</summary>
    [HttpGet("thread-pool")]
    public IActionResult ThreadPool() => NaoImplementado();

    /// <summary>Em qual thread a requisição executa?</summary>
    [HttpGet("id-da-thread")]
    public IActionResult IdDaThread() => NaoImplementado();
}
