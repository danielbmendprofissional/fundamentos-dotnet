using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Thread e ThreadPool.</summary>
[Route("api/concorrencia/threads")]
[Tags("03.01 - Threads")]
public class ThreadsController : EstudoControllerBase
{
    /// <summary>Criar e iniciar uma Thread.</summary>
    [HttpGet("criar")]
    public IActionResult Criar()
    {
        var log = new System.Collections.Concurrent.ConcurrentQueue<string>();
        int idDaRequisicao = Environment.CurrentManagedThreadId;

        var thread = new Thread(() =>
        {
            log.Enqueue($"thread nova iniciou (id {Environment.CurrentManagedThreadId})");
            Thread.Sleep(200);                          // simula trabalho
            log.Enqueue("thread nova terminou");
        });

        thread.Start();                                  // dispara e segue em frente
        log.Enqueue($"requisição continuou sem esperar (id {idDaRequisicao})");

        Thread.Sleep(400);                               // só para dar tempo de a thread terminar
        return Ok(new { idDaRequisicao, log });
    }


    /// <summary>Esperar o término de uma thread.</summary>
    [HttpGet("join")]
    public IActionResult Join()
    {
        int resultado = 0;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var thread = new Thread(() =>
        {
            Thread.Sleep(500);
            resultado = 42;
        });
        thread.Start();

        int antesDoJoin = resultado;                     // quase sempre 0: a thread ainda trabalha
        thread.Join();                                   // BLOQUEIA até a thread terminar
        int depoisDoJoin = resultado;

        return Ok(new { antesDoJoin, depoisDoJoin, esperouMs = sw.ElapsedMilliseconds });
    }


    /// <summary>ThreadPool.QueueUserWorkItem x new Thread.</summary>
    [HttpGet("thread-pool")]
    public IActionResult ThreadPoolEndpoint()
    {
        const int tarefas = 20;
        var idsPool = new System.Collections.Concurrent.ConcurrentBag<int>();
        using var terminou = new CountdownEvent(tarefas);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < tarefas; i++)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                idsPool.Add(Environment.CurrentManagedThreadId);
                terminou.Signal();
            });
        }
        terminou.Wait();
        double msPool = sw.Elapsed.TotalMilliseconds;

        var idsNew = new System.Collections.Concurrent.ConcurrentBag<int>();
        sw.Restart();
        var threads = Enumerable.Range(0, tarefas).Select(_ => new Thread(() => idsNew.Add(Environment.CurrentManagedThreadId))).ToList();
        threads.ForEach(t => t.Start());
        threads.ForEach(t => t.Join());
        double msNew = sw.Elapsed.TotalMilliseconds;

        ThreadPool.GetAvailableThreads(out int livres, out _);
        ThreadPool.GetMaxThreads(out int maximo, out _);

        return Ok(new
        {
            pool = new { tarefas, threadsDiferentesUsadas = idsPool.Distinct().Count(), ms = msPool },
            newThread = new { tarefas, threadsDiferentesUsadas = idsNew.Distinct().Count(), ms = msNew },
            threadsDoPoolEmUso = maximo - livres
        });
    }


    /// <summary>Em qual thread a requisição executa?</summary>
    [HttpGet("id-da-thread")]
    public async Task<IActionResult> IdDaThread()
    {
        int antes = Environment.CurrentManagedThreadId;
        bool eDoPool = Thread.CurrentThread.IsThreadPoolThread;

        await Task.Delay(100);                           // libera a thread enquanto espera

        int depois = Environment.CurrentManagedThreadId;

        return Ok(new { antes, depois, mudouDeThread = antes != depois, eDoPool });
    }

}
