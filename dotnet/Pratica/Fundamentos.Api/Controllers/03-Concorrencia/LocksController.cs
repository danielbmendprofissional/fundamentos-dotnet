using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fundamentos.Api.Controllers;

/// <summary>Sincronização de acesso a estado compartilhado.</summary>
[Route("api/concorrencia/locks")]
[Tags("03.03 - Locks e race conditions")]
public class LocksController : EstudoControllerBase
{
    /// <summary>Contador sem proteção: observar o resultado errado.</summary>
    [HttpGet("race-condition")]
    public IActionResult RaceCondition()
    {
        int contador = 0;
        Disparar(() => contador++);                      // sem proteção

        int esperado = Threads * VoltasPorThread;
        return Ok(new { esperado, obtido = contador, somasPerdidas = esperado - contador });
    }


    /// <summary>Corrigir com lock.</summary>
    [HttpGet("lock")]
    public IActionResult Lock()
    {
        int contador = 0;
        var trava = new object();
        Disparar(() => { lock (trava) { contador++; } });

        int esperado = Threads * VoltasPorThread;
        return Ok(new { esperado, obtido = contador });
    }


    /// <summary>Incremento atômico.</summary>
    [HttpGet("interlocked")]
    public IActionResult InterlockedEndpoint()
    {
        int contador = 0;
        Disparar(() => Interlocked.Increment(ref contador));

        int esperado = Threads * VoltasPorThread;
        return Ok(new { esperado, obtido = contador });
    }


    /// <summary>Limitar concorrência (funciona com async).</summary>
    [HttpGet("semaphore-slim")]
    public async Task<IActionResult> SemaphoreSlimEndpoint()
    {
        using var semaforo = new SemaphoreSlim(3);       // no máximo 3 ao mesmo tempo
        int emExecucao = 0, pico = 0;

        var tarefas = Enumerable.Range(0, 10).Select(async _ =>
        {
            await semaforo.WaitAsync();                  // espera uma vaga SEM bloquear a thread
            try
            {
                int agora = Interlocked.Increment(ref emExecucao);
                int atual;
                while (agora > (atual = Volatile.Read(ref pico)))
                    Interlocked.CompareExchange(ref pico, agora, atual);

                await Task.Delay(200);                   // trabalho (ex.: chamar API externa)
                Interlocked.Decrement(ref emExecucao);
            }
            finally { semaforo.Release(); }              // sempre devolve a vaga
        });

        var sw = Stopwatch.StartNew();
        await Task.WhenAll(tarefas);

        return Ok(new { tarefas = 10, limite = 3, picoSimultaneo = pico, ms = sw.ElapsedMilliseconds });
    }


    /// <summary>Provocar e entender um deadlock.</summary>
    [HttpGet("deadlock")]
    public IActionResult Deadlock()
    {
        var a = new object();
        var b = new object();
        var detectado = new bool[2];
        using var ambasPegaramPrimeira = new Barrier(2);

        var t1 = Task.Run(() =>
        {
            lock (a)
            {
                ambasPegaramPrimeira.SignalAndWait();    // garante que as duas já seguram a 1ª trava
                if (!Monitor.TryEnter(b, 1000))          // quer a b, que a outra segura
                    detectado[0] = true;
                else Monitor.Exit(b);
            }
        });

        var t2 = Task.Run(() =>
        {
            lock (b)
            {
                ambasPegaramPrimeira.SignalAndWait();
                if (!Monitor.TryEnter(a, 1000))          // quer a a, que a outra segura
                    detectado[1] = true;
                else Monitor.Exit(a);
            }
        });

        Task.WaitAll(t1, t2);
        return Ok(new { thread1PresaEsperandoB = detectado[0], thread2PresaEsperandoA = detectado[1] });
    }



    private const int Threads = 8;
    private const int VoltasPorThread = 100_000;

    private static void Disparar(Action trabalho)
    {
        var tarefas = Enumerable.Range(0, Threads).Select(_ => Task.Run(() =>
        {
            for (int i = 0; i < VoltasPorThread; i++) trabalho();
        }));
        Task.WaitAll(tarefas);
    }

}
