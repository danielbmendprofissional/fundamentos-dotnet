using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>List&lt;T&gt;: array dinâmico que cresce sozinho.</summary>
[Route("api/colecoes/listas")]
[Tags("01.03 - Listas")]
public class ListasController : EstudoControllerBase
{

    private static readonly List<string> _times = new();

    /// <summary>Listar itens da lista.</summary>
    [HttpGet]
    public IActionResult Listar() =>
        Ok(new { times = _times, count = _times.Count });


    /// <summary>Add: custo e crescimento da capacidade.</summary>
    [HttpPost]
    public IActionResult Adicionar([FromQuery] string nome)
    {
        _times.Add(nome);
        return Ok(new { times = _times, count = _times.Count });
    }


    /// <summary>Insert no meio: custo O(n).</summary>
    [HttpPost("inserir")]
    public IActionResult Inserir([FromQuery] int posicao, [FromQuery] string nome)
    {
        //inserir um valor bem no meio de uma lista, em uma posição mudando a posição de todos

        var antes = _times.ToList();
        _times.Insert(posicao, nome);
        return Ok(new { antes, depois = _times });
    }


    /// <summary>Remove, RemoveAt e RemoveAll.</summary>
    [HttpDelete("remover")]
    public IActionResult Remover([FromQuery] string nome)
    {
        //remove um item da lista e reordena a posição de todos

        bool removeu = _times.Remove(nome);            // false se não existir
        return Ok(new { removeu, times = _times });
    }


    /// <summary>Count x Capacity: observar o dobro da capacidade a cada estouro.</summary>
    [HttpGet("capacidade")]
    public IActionResult Capacidade()
    {
        var lista = new List<int>();
        var passos = new List<object> { new { count = lista.Count, capacity = lista.Capacity } };

        for (int i = 1; i <= 10; i++)
        {
            lista.Add(i);
            passos.Add(new { count = lista.Count, capacity = lista.Capacity });
        }

        return Ok(passos);
    }


    /// <summary>Sort, Find, FindAll, Exists.</summary>
    [HttpGet("ordenar-e-filtrar")]
    public IActionResult OrdenarEFiltrar()
    {
        var nomes = new List<string> { "Hulk", "Scarpa", "Paulinho", "Everson", "Rony" };
        var original = nomes.ToList();                                // cópia para mostrar o "antes"

        nomes.Sort();                                                 // ordena a própria lista

        string? primeiroComP = nomes.Find(n => n.StartsWith("P"));    // primeiro que bate, ou null
        List<string> comLetraO = nomes.FindAll(n => n.Contains('o')); // todos que batem
        bool existeHulk = nomes.Exists(n => n == "Hulk");             // true ou false
        bool contemRony = nomes.Contains("Rony");                     // comparação direta

        return Ok(new { original, ordenada = nomes, primeiroComP, comLetraO, existeHulk, contemRony });
    }
}
