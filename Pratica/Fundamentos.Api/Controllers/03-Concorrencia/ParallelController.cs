using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fundamentos.Api.Controllers;

/// <summary>Parallel e PLINQ: trabalho CPU-bound.</summary>
[Route("api/concorrencia/paralelismo")]
[Tags("03.05 - Paralelismo")]
public class ParallelController : EstudoControllerBase
{
    /// <summary>Parallel.For e Parallel.ForEach.</summary>
    [HttpGet("parallel-for")]
    public IActionResult ParallelFor()
    {
        const int limite = 3_000_000;

        var sw = Stopwatch.StartNew();
        int sequencial = 0;
        for (int n = 0; n < limite; n++)
            if (EhPrimo(n)) sequencial++;
        long msSeq = sw.ElapsedMilliseconds;

        sw.Restart();
        int paralelo = 0;
        Parallel.For(0, limite, n =>
        {
            if (EhPrimo(n)) Interlocked.Increment(ref paralelo);   // contador compartilhado: precisa ser atômico
        });
        long msPar = sw.ElapsedMilliseconds;

        return Ok(new { nucleos = Environment.ProcessorCount, sequencial, paralelo, msSequencial = msSeq, msParalelo = msPar });
    }


    /// <summary>AsParallel.</summary>
    [HttpGet("plinq")]
    public IActionResult Plinq()
    {
        var numeros = Enumerable.Range(0, 3_000_000);

        var sw = Stopwatch.StartNew();
        int comum = numeros.Count(EhPrimo);
        long msComum = sw.ElapsedMilliseconds;

        sw.Restart();
        int paralela = numeros.AsParallel().Count(EhPrimo);
        long msParalela = sw.ElapsedMilliseconds;

        var primeirosDez = numeros.AsParallel().AsOrdered().Where(EhPrimo).Take(10).ToList();

        return Ok(new { comum, paralela, msComum, msParalela, primeirosDez });
    }


    /// <summary>Quando async (I/O) e quando paralelo (CPU).</summary>
    [HttpGet("io-vs-cpu")]
    public async Task<IActionResult> IoVsCpu()
    {
        const int tarefas = 20;
        var sw = Stopwatch.StartNew();

        // I/O simulado com Delay: async (não ocupa thread)
        await Task.WhenAll(Enumerable.Range(0, tarefas).Select(_ => Task.Delay(300)));
        long ioAsync = sw.ElapsedMilliseconds;

        // I/O com Parallel.For + Sleep: ocupa threads à toa
        sw.Restart();
        Parallel.For(0, tarefas, _ => Thread.Sleep(300));
        long ioParalelo = sw.ElapsedMilliseconds;

        // CPU em sequência
        sw.Restart();
        for (int i = 0; i < tarefas; i++) EhPrimo(2_147_483_647);
        long cpuSeq = sw.ElapsedMilliseconds;

        // CPU em paralelo
        sw.Restart();
        Parallel.For(0, tarefas, _ => EhPrimo(2_147_483_647));
        long cpuPar = sw.ElapsedMilliseconds;

        // CPU em async: não ajuda em nada
        sw.Restart();
        for (int i = 0; i < tarefas; i++) await Task.FromResult(EhPrimo(2_147_483_647));
        long cpuAsync = sw.ElapsedMilliseconds;

        return Ok(new { ioAsync, ioParalelo, cpuSeq, cpuPar, cpuAsync });
    }


    private static bool EhPrimo(int n)
    {
        if (n < 2) return false;
        for (int i = 2; (long)i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

}
