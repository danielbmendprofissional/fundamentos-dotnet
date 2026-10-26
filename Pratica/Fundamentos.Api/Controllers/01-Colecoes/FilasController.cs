using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Queue&lt;T&gt;: FIFO (primeiro a entrar, primeiro a sair).</summary>
[Route("api/colecoes/filas")]
[Tags("01.07 - Filas")]
public class FilasController : EstudoControllerBase
{
    private static readonly Queue<string> _fila = new();
    /// <summary>Enqueue.</summary>
    [HttpPost("enfileirar")]
    public IActionResult Enfileirar([FromQuery] string pessoa)
    {
        _fila.Enqueue(pessoa);
        return Ok(new { proximo = _fila.Peek(), fila = _fila, count = _fila.Count });
    }

    /// <summary>Dequeue. E se a fila estiver vazia?</summary>
    [HttpPost("desenfileirar")]
    public IActionResult Desenfileirar()
    {
        if (!_fila.TryDequeue(out string? atendido))
            return Ok(new { atendido = (string?)null, mensagem = "fila vazia" });

        return Ok(new { atendido, fila = _fila, count = _fila.Count });
    }

    /// <summary>Simular atendimento.</summary>
    [HttpGet("fila-de-atendimento")]
    public IActionResult FilaDeAtendimento()
    {
        var fila = new Queue<string>();
        var log = new List<string>();

        // "+Nome" = chegou, "-" = o guichê chama o próximo
        string[] eventos = { "+Ana", "+Bruno", "-", "+Carla", "-", "-" };

        foreach (string evento in eventos)
        {
            if (evento.StartsWith('+'))
            {
                string nome = evento[1..];
                fila.Enqueue(nome);
                log.Add($"{nome} chegou");
            }
            else if (fila.TryDequeue(out string? atendido))
            {
                log.Add($"{atendido} foi atendido");
            }
        }

        return Ok(log);
    }


    /// <summary>PriorityQueue&lt;TElement,TPriority&gt;.</summary>
    [HttpGet("prioridade")]
    public IActionResult Prioridade()
    {
        var fila = new PriorityQueue<string, int>();

        // quanto MENOR o número, MAIOR a prioridade
        fila.Enqueue("Comum", 3);
        fila.Enqueue("Idoso", 1);
        fila.Enqueue("Sócio-torcedor", 2);

        var ordemDeSaida = new List<string>();
        while (fila.TryDequeue(out string? pessoa, out int prioridade))
            ordemDeSaida.Add($"{pessoa} (prioridade {prioridade})");

        return Ok(ordemDeSaida);
    }

}
