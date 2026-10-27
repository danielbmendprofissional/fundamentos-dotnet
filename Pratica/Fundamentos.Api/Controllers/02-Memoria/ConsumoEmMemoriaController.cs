using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Quanto dado você traz para a RAM: carregar tudo x processar em fluxo.</summary>
[Route("api/memoria/consumo")]
[Tags("02.03 - Consumo em memória")]
public class ConsumoEmMemoriaController : EstudoControllerBase
{
    /// <summary>File.ReadAllLines x File.ReadLines: memória e tempo.</summary>
    [HttpGet("carregar-tudo-vs-fluxo")]
    public IActionResult CarregarTudoVsFluxo() => NaoImplementado();

    /// <summary>ToList (tudo na RAM) x IEnumerable com yield (um por vez).</summary>
    [HttpGet("materializar-vs-streaming")]
    public IActionResult MaterializarVsStreaming() => NaoImplementado();

    /// <summary>Skip e Take: nunca trazer a tabela inteira.</summary>
    [HttpGet("paginacao")]
    public IActionResult Paginacao() => NaoImplementado();

    /// <summary>Onde o filtro roda: no SQL (IQueryable) ou no C# depois do ToList.</summary>
    [HttpGet("iqueryable-vs-ienumerable")]
    public IActionResult IQueryableVsIEnumerable() => NaoImplementado();

    /// <summary>Por que o provider InMemory do EF engana nos testes (sem constraints, sem transações).</summary>
    [HttpGet("ef-inmemory-vs-banco-real")]
    public IActionResult EfInMemoryVsBancoReal() => NaoImplementado();
}
