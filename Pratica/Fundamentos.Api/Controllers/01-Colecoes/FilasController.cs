using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Queue&lt;T&gt;: FIFO (primeiro a entrar, primeiro a sair).</summary>
[Route("api/colecoes/filas")]
[Tags("01.07 - Filas")]
public class FilasController : EstudoControllerBase
{
    /// <summary>Enqueue.</summary>
    [HttpPost("enfileirar")]
    public IActionResult Enfileirar() => NaoImplementado();

    /// <summary>Dequeue. E se a fila estiver vazia?</summary>
    [HttpPost("desenfileirar")]
    public IActionResult Desenfileirar() => NaoImplementado();

    /// <summary>Simular atendimento.</summary>
    [HttpGet("fila-de-atendimento")]
    public IActionResult FilaDeAtendimento() => NaoImplementado();

    /// <summary>PriorityQueue&lt;TElement,TPriority&gt;.</summary>
    [HttpGet("prioridade")]
    public IActionResult Prioridade() => NaoImplementado();
}
