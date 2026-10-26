using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Stack&lt;T&gt;: LIFO (último a entrar, primeiro a sair).</summary>
[Route("api/colecoes/pilhas")]
[Tags("01.06 - Pilhas")]
public class PilhasController : EstudoControllerBase
{
    /// <summary>Empilhar.</summary>
    [HttpPost("push")]
    public IActionResult Push() => NaoImplementado();

    /// <summary>Desempilhar. E se a pilha estiver vazia?</summary>
    [HttpPost("pop")]
    public IActionResult Pop() => NaoImplementado();

    /// <summary>Espiar o topo sem remover.</summary>
    [HttpGet("peek")]
    public IActionResult Peek() => NaoImplementado();

    /// <summary>Validar parênteses balanceados com pilha.</summary>
    [HttpGet("parenteses-balanceados")]
    public IActionResult ParentesesBalanceados() => NaoImplementado();

    /// <summary>Undo de operações.</summary>
    [HttpPost("desfazer")]
    public IActionResult Desfazer() => NaoImplementado();
}
