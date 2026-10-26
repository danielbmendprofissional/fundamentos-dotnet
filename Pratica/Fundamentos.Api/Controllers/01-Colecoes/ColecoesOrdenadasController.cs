using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>SortedDictionary, SortedSet, SortedList: ordem mantida a cada inserção.</summary>
[Route("api/colecoes/ordenadas")]
[Tags("01.09 - Coleções ordenadas")]
public class ColecoesOrdenadasController : EstudoControllerBase
{
    /// <summary>Chaves sempre ordenadas.</summary>
    [HttpGet("sorted-dictionary")]
    public IActionResult SortedDictionary() => NaoImplementado();

    /// <summary>Conjunto ordenado, GetViewBetween.</summary>
    [HttpGet("sorted-set")]
    public IActionResult SortedSet() => NaoImplementado();

    /// <summary>Comparar o custo das duas estratégias.</summary>
    [HttpGet("ordenar-no-fim-vs-sempre-ordenado")]
    public IActionResult OrdenarNoFimVsSempreOrdenado() => NaoImplementado();
}
