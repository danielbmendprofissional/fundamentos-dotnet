using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>struct x class, stack x heap, boxing.</summary>
[Route("api/memoria/valor-vs-referencia")]
[Tags("02.02 - Valor x referência")]
public class ValorVsReferenciaController : EstudoControllerBase
{
    /// <summary>Copiar um struct e uma class e alterar o original.</summary>
    [HttpGet("struct-vs-class")]
    public IActionResult StructVsClass() => NaoImplementado();

    /// <summary>Boxing e unboxing e seu custo.</summary>
    [HttpGet("boxing")]
    public IActionResult Boxing() => NaoImplementado();

    /// <summary>ref, out, in.</summary>
    [HttpGet("passagem-de-parametros")]
    public IActionResult PassagemDeParametros() => NaoImplementado();

    /// <summary>Imutabilidade e StringBuilder.</summary>
    [HttpGet("string-imutavel")]
    public IActionResult StringImutavel() => NaoImplementado();
}
