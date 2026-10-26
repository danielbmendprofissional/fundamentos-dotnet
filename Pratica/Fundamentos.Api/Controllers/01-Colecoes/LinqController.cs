using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>LINQ sobre coleções.</summary>
[Route("api/colecoes/linq")]
[Tags("01.13 - LINQ")]
public class LinqController : EstudoControllerBase
{
    /// <summary>Where e Select.</summary>
    [HttpGet("filtrar-e-projetar")]
    public IActionResult FiltrarEProjetar() => NaoImplementado();

    /// <summary>GroupBy, ToDictionary, ToLookup.</summary>
    [HttpGet("agrupar")]
    public IActionResult Agrupar() => NaoImplementado();

    /// <summary>Deferred execution x ToList (materializar).</summary>
    [HttpGet("execucao-adiada")]
    public IActionResult ExecucaoAdiada() => NaoImplementado();

    /// <summary>yield return e iteradores.</summary>
    [HttpGet("yield")]
    public IActionResult Yield() => NaoImplementado();
}
