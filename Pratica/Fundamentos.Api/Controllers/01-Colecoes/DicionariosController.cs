using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Dictionary&lt;TKey,TValue&gt;: tabela hash, busca por chave.</summary>
[Route("api/colecoes/dicionarios")]
[Tags("01.04 - Dicionários")]
public class DicionariosController : EstudoControllerBase
{
    /// <summary>Indexador x TryGetValue (KeyNotFoundException).</summary>
    [HttpGet("buscar")]
    public IActionResult Buscar() => NaoImplementado();

    /// <summary>Add x indexador x TryAdd (chave duplicada).</summary>
    [HttpPost("adicionar")]
    public IActionResult Adicionar() => NaoImplementado();

    /// <summary>Contar a frequência de palavras de um texto.</summary>
    [HttpGet("frequencia-palavras")]
    public IActionResult FrequenciaPalavras() => NaoImplementado();

    /// <summary>Equals e GetHashCode: usar um Value Object como chave.</summary>
    [HttpGet("chave-customizada")]
    public IActionResult ChaveCustomizada() => NaoImplementado();
}
