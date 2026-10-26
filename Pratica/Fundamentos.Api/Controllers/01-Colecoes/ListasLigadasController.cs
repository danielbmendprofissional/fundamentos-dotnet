using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>LinkedList&lt;T&gt;: nós encadeados.</summary>
[Route("api/colecoes/listas-ligadas")]
[Tags("01.08 - Listas ligadas")]
public class ListasLigadasController : EstudoControllerBase
{
    /// <summary>AddFirst e AddLast: O(1).</summary>
    [HttpPost("inserir-inicio-fim")]
    public IActionResult InserirInicioFim() => NaoImplementado();

    /// <summary>Navegar por nós (Next e Previous).</summary>
    [HttpGet("percorrer")]
    public IActionResult Percorrer() => NaoImplementado();

    /// <summary>Voltar e avançar.</summary>
    [HttpGet("historico-de-navegacao")]
    public IActionResult HistoricoDeNavegacao() => NaoImplementado();

    /// <summary>Quando usar e quando não usar: comparar com List&lt;T&gt;.</summary>
    [HttpGet("list-vs-linkedlist")]
    public IActionResult ListVsLinkedList() => NaoImplementado();
}
