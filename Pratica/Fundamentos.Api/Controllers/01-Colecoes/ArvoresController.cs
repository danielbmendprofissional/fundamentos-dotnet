using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Árvore binária de busca, implementada à mão (não existe Tree&lt;T&gt; pronta).</summary>
[Route("api/colecoes/arvores")]
[Tags("01.10 - Árvores")]
public class ArvoresController : EstudoControllerBase
{
    /// <summary>Inserir um nó na BST.</summary>
    [HttpPost("inserir")]
    public IActionResult Inserir() => NaoImplementado();

    /// <summary>Buscar um valor.</summary>
    [HttpGet("buscar")]
    public IActionResult Buscar() => NaoImplementado();

    /// <summary>In-order.</summary>
    [HttpGet("percurso/em-ordem")]
    public IActionResult PercursoEmOrdem() => NaoImplementado();

    /// <summary>Pre-order.</summary>
    [HttpGet("percurso/pre-ordem")]
    public IActionResult PercursoPreOrdem() => NaoImplementado();

    /// <summary>Post-order.</summary>
    [HttpGet("percurso/pos-ordem")]
    public IActionResult PercursoPosOrdem() => NaoImplementado();

    /// <summary>Level-order (usa fila).</summary>
    [HttpGet("percurso/em-largura")]
    public IActionResult PercursoEmLargura() => NaoImplementado();

    /// <summary>Calcular altura (recursão).</summary>
    [HttpGet("altura")]
    public IActionResult Altura() => NaoImplementado();
}
