using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Grafo: vértices e arestas (lista de adjacência).</summary>
[Route("api/colecoes/grafos")]
[Tags("01.11 - Grafos")]
public class GrafosController : EstudoControllerBase
{
    /// <summary>Adicionar vértice e aresta.</summary>
    [HttpPost("aresta")]
    public IActionResult AdicionarAresta() => NaoImplementado();

    /// <summary>BFS (usa fila).</summary>
    [HttpGet("bfs")]
    public IActionResult BuscaEmLargura() => NaoImplementado();

    /// <summary>DFS (usa pilha ou recursão).</summary>
    [HttpGet("dfs")]
    public IActionResult BuscaEmProfundidade() => NaoImplementado();

    /// <summary>Menor caminho em grafo sem peso.</summary>
    [HttpGet("menor-caminho")]
    public IActionResult MenorCaminho() => NaoImplementado();
}
