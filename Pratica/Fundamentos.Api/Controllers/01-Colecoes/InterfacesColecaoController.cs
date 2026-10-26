using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>IEnumerable, ICollection, IList, IReadOnlyList: contratos, não implementações.</summary>
[Route("api/colecoes/interfaces")]
[Tags("01.12 - Interfaces de coleção")]
public class InterfacesColecaoController : EstudoControllerBase
{
    /// <summary>Quem herda de quem e o que cada interface oferece.</summary>
    [HttpGet("hierarquia")]
    public IActionResult Hierarquia() => NaoImplementado();

    /// <summary>IReadOnlyCollection x List exposta (como o Galo protege agregados).</summary>
    [HttpGet("somente-leitura")]
    public IActionResult SomenteLeitura() => NaoImplementado();

    /// <summary>InvalidOperationException ao alterar a coleção durante o foreach.</summary>
    [HttpGet("modificar-no-foreach")]
    public IActionResult ModificarNoForeach() => NaoImplementado();
}
