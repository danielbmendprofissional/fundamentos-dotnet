using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Árvore binária de busca, implementada à mão (não existe Tree&lt;T&gt; pronta).</summary>
[Route("api/colecoes/arvores")]
[Tags("01.10 - Árvores")]
public class ArvoresController : EstudoControllerBase
{
    private static readonly ArvoreBinariaBusca _arvore = new();


    /// <summary>Inserir um nó na BST.</summary>
    [HttpPost("inserir")]
    public IActionResult Inserir([FromQuery] int valor)
    {
        bool inseriu = _arvore.Inserir(valor);
        return Ok(new { valor, inseriu, emOrdem = _arvore.EmOrdem() });
    }

    /// <summary>Buscar um valor.</summary>
    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] int valor)
    {
        bool encontrado = _arvore.Buscar(valor, out int passos);
        return Ok(new { valor, encontrado, passos });
    }

    /// <summary>In-order.</summary>
    [HttpGet("percurso/em-ordem")]
    public IActionResult PercursoEmOrdem() => Ok(_arvore.EmOrdem());

    /// <summary>Pre-order.</summary>
    [HttpGet("percurso/pre-ordem")]
    public IActionResult PercursoPreOrdem() => Ok(_arvore.PreOrdem());

    /// <summary>Post-order.</summary>
    [HttpGet("percurso/pos-ordem")]
    public IActionResult PercursoPosOrdem() => Ok(_arvore.PosOrdem());

    /// <summary>Level-order (usa fila).</summary>
    [HttpGet("percurso/em-largura")]
    public IActionResult PercursoEmLargura() => Ok(_arvore.EmLargura());

    /// <summary>Calcular altura (recursão).</summary>
    [HttpGet("altura")]
    public IActionResult Altura() => Ok(new { altura = _arvore.Altura() });
}
