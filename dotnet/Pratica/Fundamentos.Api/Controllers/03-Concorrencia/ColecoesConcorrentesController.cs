using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>System.Collections.Concurrent e Channels.</summary>
[Route("api/concorrencia/colecoes-concorrentes")]
[Tags("03.04 - Coleções concorrentes")]
public class ColecoesConcorrentesController : EstudoControllerBase
{
    private const int Threads = 8;
    private const int Itens = 10_000;


    /// <summary>List&lt;T&gt; acessada por várias threads: o que quebra.</summary>
    [HttpGet("list-sem-protecao")]
    public IActionResult ListSemProtecao()
    {
        //Ou da erro ou valores são perdidos sem nem ver

        var lista = new List<int>();
        string? erro = null;
        try
        {
            Parallel.For(0, Threads * Itens, i => lista.Add(i));
        }
        catch (AggregateException ex)
        {
            erro = ex.InnerException?.GetType().Name;
        }

        int esperado = Threads * Itens;
        return Ok(new { esperado, obtido = lista.Count, perdidos = esperado - lista.Count, erro });
    }


    /// <summary>AddOrUpdate e GetOrAdd.</summary>
    [HttpGet("concurrent-dictionary")]
    public IActionResult ConcurrentDictionaryEndpoint()
    {
        var contagem = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        string[] posicoes = { "Atacante", "Meia", "Zagueiro", "Goleiro" };

        Parallel.For(0, 40_000, i =>
        {
            string chave = posicoes[i % posicoes.Length];
            contagem.AddOrUpdate(chave, 1, (_, atual) => atual + 1);   // soma de forma atômica
        });

        var cache = new System.Collections.Concurrent.ConcurrentDictionary<int, string>();
        string primeiro = cache.GetOrAdd(1, k => $"valor {k}");        // cria se não existe
        string segundo = cache.GetOrAdd(1, k => "outro");              // já existe: devolve o antigo

        return Ok(new { contagem, primeiro, segundo });
    }


    /// <summary>Fila segura entre threads.</summary>
    [HttpGet("concurrent-queue")]
    public IActionResult ConcurrentQueueEndpoint()
    {
        var fila = new System.Collections.Concurrent.ConcurrentQueue<int>();
        var consumidos = new System.Collections.Concurrent.ConcurrentBag<int>();

        Parallel.For(0, 10_000, i => fila.Enqueue(i));                   // várias threads enfileiram

        Parallel.For(0, Threads, _ =>                                     // várias threads consomem
        {
            while (fila.TryDequeue(out int item))                         // não há "Dequeue" que lance
                consumidos.Add(item);
        });

        return Ok(new { consumidos = consumidos.Count, repetidos = consumidos.Count - consumidos.Distinct().Count(), sobrou = fila.Count });
    }


    /// <summary>Channel&lt;T&gt; ou BlockingCollection&lt;T&gt;.</summary>
    [HttpGet("produtor-consumidor")]
    public async Task<IActionResult> ProdutorConsumidor()
    {
        var canal = System.Threading.Channels.Channel.CreateBounded<int>(5);   // buffer de 5: aplica "backpressure"
        var log = new System.Collections.Concurrent.ConcurrentQueue<string>();

        var produtor = Task.Run(async () =>
        {
            for (int i = 1; i <= 10; i++)
            {
                await canal.Writer.WriteAsync(i);          // se o buffer encher, ESPERA (sem travar thread)
                log.Enqueue($"produziu {i}");
            }
            canal.Writer.Complete();                       // avisa: não vem mais nada
        });

        var consumidor = Task.Run(async () =>
        {
            await foreach (int item in canal.Reader.ReadAllAsync())
            {
                await Task.Delay(50);                      // consumidor mais lento que o produtor
                log.Enqueue($"consumiu {item}");
            }
        });

        await Task.WhenAll(produtor, consumidor);
        return Ok(new { itens = log.Count, log });
    }

}
