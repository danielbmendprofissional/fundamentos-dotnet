using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Fundamentos.Api.Controllers;

/// <summary>Gerenciamento automático do heap: gerações, LOH, coleta e vazamento.</summary>
[Route("api/fundamentos/garbage-collector")]
[Tags("00.04 - Garbage Collector")]
public class GarbageCollectorController : EstudoControllerBase
{
    /// <summary>Gen 0, 1 e 2: o objeto nasce na 0 e é promovido ao sobreviver (GC.GetGeneration).</summary>
    [HttpGet("geracoes")]
    public IActionResult Geracoes()
    {
        var objeto = new object();                       // referenciado por uma variável: está vivo

        int aoCriar = GC.GetGeneration(objeto);
        GC.Collect();
        int aposPrimeiraColeta = GC.GetGeneration(objeto);
        GC.Collect();
        int aposSegundaColeta = GC.GetGeneration(objeto);
        GC.Collect();
        int aposTerceiraColeta = GC.GetGeneration(objeto);

        GC.KeepAlive(objeto);                            // garante que ele viveu até aqui
        return Ok(new { aoCriar, aposPrimeiraColeta, aposSegundaColeta, aposTerceiraColeta, geracaoMaxima = GC.MaxGeneration });
    }


    /// <summary>GC.Collect e GC.CollectionCount antes e depois (e por que não usar em produção).</summary>
    private static object? _sink;                        // impede o JIT de otimizar a alocação

    [HttpGet("coleta-manual")]
    public IActionResult ColetaManual()
    {
        int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);

        for (int i = 0; i < 200_000; i++)
            _sink = new byte[1_000];                     // cada volta descarta o array anterior

        int g0b = GC.CollectionCount(0), g1b = GC.CollectionCount(1), g2b = GC.CollectionCount(2);

        GC.Collect();                                    // forçado, só para comparar

        return Ok(new
        {
            coletasNaturais = new { gen0 = g0b - g0, gen1 = g1b - g1, gen2 = g2b - g2 },
            coletaManual = new
            {
                gen0 = GC.CollectionCount(0) - g0b,
                gen1 = GC.CollectionCount(1) - g1b,
                gen2 = GC.CollectionCount(2) - g2b
            }
        });
    }


    /// <summary>GC.GetTotalMemory ao alocar e liberar uma lista grande.</summary>
    private static double MB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0, 1);

    [HttpGet("memoria-antes-e-depois")]
    public IActionResult MemoriaAntesEDepois()
    {
        long inicio = GC.GetTotalMemory(true);           // true = coleta antes, para medir "limpo"

        var grande = new List<byte[]>();
        for (int i = 0; i < 5_000; i++)
            grande.Add(new byte[10_000]);                // cerca de 50 MB
        long comLista = GC.GetTotalMemory(false);

        grande = null!;                                  // ninguém mais referencia a lista
        long semColetar = GC.GetTotalMemory(false);      // o GC ainda não rodou
        long depoisDaColeta = GC.GetTotalMemory(true);   // força a coleta e mede

        return Ok(new
        {
            inicioMB = MB(inicio),
            comListaMB = MB(comLista),
            semColetarMB = MB(semColetar),
            depoisDaColetaMB = MB(depoisDaColeta)
        });
    }


    /// <summary>Objetos a partir de ~85 mil bytes vão direto ao LOH (gen 2).</summary>
    [HttpGet("large-object-heap")]
    public IActionResult LargeObjectHeap()
    {
        var pequeno = new byte[80_000];                  // abaixo de ~85 mil bytes
        var grande = new byte[90_000];                   // acima: vai para o LOH

        return Ok(new
        {
            geracaoDoPequeno = GC.GetGeneration(pequeno),
            geracaoDoGrande = GC.GetGeneration(grande)
        });
    }


    /// <summary>Coleção estática que só cresce: o GC não coleta o que ainda é referenciado.</summary>
    private static readonly List<byte[]> _vazados = new();

    [HttpGet("vazamento-de-memoria")]
    public IActionResult VazamentoDeMemoria([FromQuery] int mb = 10, [FromQuery] bool limpar = false)
    {
        const int limiteMb = 200;

        if (limpar)
            _vazados.Clear();
        else if (_vazados.Count + mb > limiteMb)
            return BadRequest(new { mensagem = $"limite de {limiteMb} MB para o estudo; use limpar=true" });
        else
            for (int i = 0; i < mb; i++)
                _vazados.Add(new byte[1_000_000]);       // 1 MB por item

        long antes = GC.GetTotalMemory(false);
        GC.Collect();
        long depois = GC.GetTotalMemory(true);

        return Ok(new { itensGuardados = _vazados.Count, heapAntesMB = MB(antes), heapDepoisDaColetaMB = MB(depois) });
    }


    /// <summary>WeakReference: referenciar sem impedir a coleta.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CriarFraca() => new WeakReference(new byte[1_000]);

    [HttpGet("referencia-fraca")]
    public IActionResult ReferenciaFraca()
    {
        var fraca = CriarFraca();                        // ninguém mais referencia o array
        bool vivoAntes = fraca.IsAlive;
        GC.Collect();
        bool vivoDepois = fraca.IsAlive;

        var forte = new byte[1_000];                     // aqui há uma referência normal
        var fracaComForte = new WeakReference(forte);
        GC.Collect();
        bool vivoComForte = fracaComForte.IsAlive;
        GC.KeepAlive(forte);

        return Ok(new { vivoAntes, vivoDepois, vivoComForte });
    }

    /// <summary>GC.GetAllocatedBytesForCurrentThread: quanto uma operação aloca.</summary>
    [HttpGet("alocacoes-de-uma-operacao")]
    private static long Medir(Action acao)
    {
        acao();                                          // aquecimento: o JIT também aloca
        long antes = GC.GetAllocatedBytesForCurrentThread();
        acao();
        return GC.GetAllocatedBytesForCurrentThread() - antes;
    }

    [HttpGet("alocacoes-de-uma-operacao")]
    public IActionResult AlocacoesDeUmaOperacao()
    {
        long comLinq = Medir(() => Enumerable.Range(1, 10_000).Select(n => n * 2).ToList().Sum());
        long comLaco = Medir(() => { long soma = 0; for (int n = 1; n <= 10_000; n++) soma += n * 2; });

        return Ok(new { bytesComLinq = comLinq, bytesComLaco = comLaco });
    }
}
