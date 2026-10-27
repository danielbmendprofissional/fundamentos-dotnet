using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fundamentos.Api.Controllers;

/// <summary>Do código-fonte ao processador: compile time (Roslyn gera IL) e runtime (CLR, JIT, GC).</summary>
[Route("api/fundamentos/execucao")]
[Tags("00.01 - Execução (compile time x runtime)")]
public class ExecucaoController : EstudoControllerBase
{
    /// <summary>Etapas: .cs, IL em .dll, CLR carrega, JIT, código nativo.</summary>
    [HttpGet("etapas")]
    public IActionResult Etapas()
    {
        var assembly = typeof(ExecucaoController).Assembly;

        var etapas = new[]
        {
        new { ordem = 1, momento = "compile time", quem = "Roslyn",
              oQue = "valida sintaxe e tipos; gera IL + metadados no .dll" },
        new { ordem = 2, momento = "runtime", quem = "host do dotnet",
              oQue = "localiza e inicia o runtime (CLR)" },
        new { ordem = 3, momento = "runtime", quem = "CLR",
              oQue = "carrega o assembly na memória" },
        new { ordem = 4, momento = "runtime", quem = "JIT",
              oQue = "converte IL em código nativo, método a método, na 1ª chamada" },
        new { ordem = 5, momento = "runtime", quem = "CPU + GC",
              oQue = "executa o código nativo; o GC gerencia o heap" }
    };

        return Ok(new
        {
            assembly = assembly.GetName().Name,
            arquivo = Path.GetFileName(assembly.Location),
            etapas
        });
    }


    /// <summary>Listar os assemblies que o CLR carregou na memória (AppDomain).</summary>
    [HttpGet("assemblies-carregados")]
    public IActionResult AssembliesCarregados()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => new { nome = a.GetName().Name, versao = a.GetName().Version?.ToString() })
            .OrderBy(a => a.nome)
            .ToList();

        return Ok(new { total = assemblies.Count, assemblies });
    }


    /// <summary>Medir a 1ª chamada de um método (inclui o JIT) x as seguintes.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]        // impede o JIT de embutir o método no chamador
    private static long Somar(int n)
    {
        long soma = 0;
        for (int i = 0; i < n; i++)
            soma += i;
        return soma;
    }

    [HttpGet("jit-primeira-chamada")]
    public IActionResult JitPrimeiraChamada()
    {
        var sw = Stopwatch.StartNew();
        Somar(1000);
        double primeiraChamadaMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        Somar(1000);
        double segundaChamadaMs = sw.Elapsed.TotalMilliseconds;

        return Ok(new { primeiraChamadaMs, segundaChamadaMs });
    }

    /// <summary>Versão do runtime, SO, arquitetura, núcleos e modo do GC.</summary>
    [HttpGet("runtime-info")]
    public IActionResult RuntimeInfo() => Ok(new
    {
        runtime = RuntimeInformation.FrameworkDescription,
        versao = Environment.Version.ToString(),
        sistemaOperacional = RuntimeInformation.OSDescription,
        arquitetura = RuntimeInformation.ProcessArchitecture.ToString(),
        nucleos = Environment.ProcessorCount,
        jitAtivo = RuntimeFeature.IsDynamicCodeCompiled,
        gcDeServidor = GCSettings.IsServerGC
    });

    /// <summary>PID, working set, threads e tempo de CPU do processo (Process).</summary>
    [HttpGet("processo")]
    public IActionResult Processo()
    {
        using var p = Process.GetCurrentProcess();

        return Ok(new
        {
            pid = p.Id,
            nome = p.ProcessName,
            iniciadoEm = p.StartTime,
            tempoDeCpuMs = Math.Round(p.TotalProcessorTime.TotalMilliseconds, 1),
            memoriaFisicaMB = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 1),
            heapGerenciadoMB = Math.Round(GC.GetTotalMemory(false) / 1024.0 / 1024.0, 1),
            threads = p.Threads.Count
        });
    }
}
