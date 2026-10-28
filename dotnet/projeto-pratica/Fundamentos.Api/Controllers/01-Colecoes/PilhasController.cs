using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Stack&lt;T&gt;: LIFO (último a entrar, primeiro a sair).</summary>
[Route("api/colecoes/pilhas")]
[Tags("01.06 - Pilhas")]
public class PilhasController : EstudoControllerBase
{
    private static readonly Stack<string> _pilha = new();

    /// <summary>Empilhar.</summary>
    [HttpPost("push")]
    public IActionResult Push([FromQuery] string item)
    {
        _pilha.Push(item);
        return Ok(new { topo = _pilha.Peek(), pilha = _pilha, count = _pilha.Count });
    }


    /// <summary>Desempilhar. E se a pilha estiver vazia?</summary>
    [HttpPost("pop")]
    public IActionResult Pop()
    {
        if (!_pilha.TryPop(out string? removido))
            return Ok(new { removido = (string?)null, mensagem = "pilha vazia" });

        return Ok(new { removido, pilha = _pilha, count = _pilha.Count });
    }


    /// <summary>Espiar o topo sem remover.</summary>
    [HttpGet("peek")]
    public IActionResult Peek()
    {
        bool temTopo = _pilha.TryPeek(out string? topo);
        return Ok(new { temTopo, topo, count = _pilha.Count });
    }


    /// <summary>Validar parênteses balanceados com pilha.</summary>
    [HttpGet("parenteses-balanceados")]
    public IActionResult ParentesesBalanceados([FromQuery] string texto = "({[]})")
    {
        var pares = new Dictionary<char, char> { [')'] = '(', [']'] = '[', ['}'] = '{' };
        var abertos = new Stack<char>();

        foreach (char c in texto)
        {
            if (c is '(' or '[' or '{')
            {
                abertos.Push(c);                              // abriu: empilha
            }
            else if (pares.TryGetValue(c, out char abertura))
            {
                // fechou: o topo precisa ser o par correspondente
                if (!abertos.TryPop(out char topo) || topo != abertura)
                    return Ok(new { texto, balanceado = false });
            }
        }

        return Ok(new { texto, balanceado = abertos.Count == 0 });   // sobrou algo aberto? então não
    }


    /// <summary>Undo de operações.</summary>
    [HttpPost("desfazer")]
    public IActionResult Desfazer()
    {
        // Aqui seria tipo o ctrl z

        var historico = new Stack<string>();
        string texto = "";

        foreach (string trecho in new[] { "Galo", " é", " massa" })
        {
            historico.Push(texto);        // guarda o estado ANTES de mudar
            texto += trecho;
        }

        var passos = new List<string> { texto };
        while (historico.Count > 0)
        {
            texto = historico.Pop();      // cada Ctrl+Z volta um estado
            passos.Add(texto);
        }

        return Ok(passos);
    }

}
