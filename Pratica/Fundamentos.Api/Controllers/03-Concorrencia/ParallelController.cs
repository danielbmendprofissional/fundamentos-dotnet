using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Parallel e PLINQ: trabalho CPU-bound.</summary>
[Route("api/concorrencia/paralelismo")]
[Tags("03.05 - Paralelismo")]
public class ParallelController : EstudoControllerBase
{
    /// <summary>Parallel.For e Parallel.ForEach.</summary>
    [HttpGet("parallel-for")]
    public IActionResult ParallelFor() => NaoImplementado();

    /// <summary>AsParallel.</summary>
    [HttpGet("plinq")]
    public IActionResult Plinq() => NaoImplementado();

    /// <summary>Quando async (I/O) e quando paralelo (CPU).</summary>
    [HttpGet("io-vs-cpu")]
    public IActionResult IoVsCpu() => NaoImplementado();
}
