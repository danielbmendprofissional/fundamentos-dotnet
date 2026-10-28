using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fundamentos.Api.Controllers;

/// <summary>Task, async/await, cancelamento.</summary>
[Route("api/concorrencia/async-await")]
[Tags("03.02 - Async/Await e Tasks")]
public class AsyncAwaitController : EstudoControllerBase
{
    /// <summary>async/await com Task.Delay (não bloqueia a thread).</summary>
    [HttpGet("basico")]
    public async Task<IActionResult> Basico()
    {
        int antes = Environment.CurrentManagedThreadId;
        var sw = Stopwatch.StartNew();

        await Task.Delay(300);                           // libera a thread; não bloqueia
        int depois = Environment.CurrentManagedThreadId;

        ThreadPool.GetAvailableThreads(out int livres, out _);
        ThreadPool.GetMaxThreads(out int maximo, out _);

        return Ok(new { ms = sw.ElapsedMilliseconds, antes, depois, mudouDeThread = antes != depois, threadsEmUso = maximo - livres });
    }


    /// <summary>Executar tarefas em paralelo e aguardar todas.</summary>
    [HttpGet("when-all")]
    public async Task<IActionResult> WhenAll()
    {
        var sw = Stopwatch.StartNew();
        await TarefaAsync("A", 300); await TarefaAsync("B", 300); await TarefaAsync("C", 300);
        long emSequencia = sw.ElapsedMilliseconds;

        sw.Restart();
        string[] resultados = await Task.WhenAll(TarefaAsync("A", 300), TarefaAsync("B", 300), TarefaAsync("C", 300));
        long emParalelo = sw.ElapsedMilliseconds;

        return Ok(new { emSequenciaMs = emSequencia, emParaleloMs = emParalelo, resultados });
    }


    /// <summary>Aguardar a primeira que terminar.</summary>
    [HttpGet("when-any")]
    public async Task<IActionResult> WhenAny()
    {
        var sw = Stopwatch.StartNew();
        var lenta = TarefaAsync("lenta", 800);
        var rapida = TarefaAsync("rápida", 200);

        Task<string> primeira = await Task.WhenAny(lenta, rapida);

        return Ok(new { vencedora = await primeira, ms = sw.ElapsedMilliseconds, lentaTerminou = lenta.IsCompleted });
    }


    /// <summary>CancellationToken.</summary>
    [HttpGet("cancelamento")]
    public async Task<IActionResult> Cancelamento([FromQuery] int timeoutMs = 500)
    {
        using var cts = new CancellationTokenSource(timeoutMs);   // cancela sozinho após o prazo
        var sw = Stopwatch.StartNew();
        try
        {
            await TarefaAsync("demorada", 3000, cts.Token);
            return Ok(new { cancelada = false });
        }
        catch (OperationCanceledException)
        {
            return Ok(new { cancelada = true, ms = sw.ElapsedMilliseconds });
        }
    }


    /// <summary>Por que .Result e .Wait() são perigosos.</summary>
    [HttpGet("deadlock-sync-over-async")]
    public async Task<IActionResult> DeadlockSyncOverAsync()
    {
        ThreadPool.GetMinThreads(out int minimo, out _);
        int tarefas = minimo + 8;                        // mais bloqueios que threads iniciais do pool
        var sw = Stopwatch.StartNew();

        var bloqueantes = Enumerable.Range(0, tarefas)
            .Select(_ => Task.Run(() => TarefaAsync("x", 500).Result));   // .Result BLOQUEIA a thread
        await Task.WhenAll(bloqueantes);
        long msBloqueando = sw.ElapsedMilliseconds;

        sw.Restart();
        await Task.WhenAll(Enumerable.Range(0, tarefas).Select(_ => TarefaAsync("x", 500)));
        long msAsync = sw.ElapsedMilliseconds;

        return Ok(new { tarefas, threadsIniciaisDoPool = minimo, msBloqueando, msAsync });
    }



    private static async Task<string> TarefaAsync(string nome, int ms, CancellationToken ct = default)
    {
        await Task.Delay(ms, ct);                        // simula I/O: não ocupa thread
        return nome;
    }
}
