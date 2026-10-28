using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Fundamentos.Api.Controllers;

/// <summary>IDisposable: o GC cuida da memória, mas não de arquivos, conexões e sockets.</summary>
[Route("api/fundamentos/dispose")]
[Tags("00.05 - Dispose e recursos")]
public class DisposeController : EstudoControllerBase
{
    /// <summary>IDisposable e using: liberação ao fim do escopo.</summary>
    private static void ComUsingDeclaration(List<string> log)
    {
        using var b = new Recurso("B", log);          // 'using var': libera no fim do escopo (o método)
        b.Usar();
        log.Add("fim do método (B ainda não foi liberado)");
    }

    [HttpGet("using")]
    public IActionResult Using()
    {
        var log = new List<string>();

        using (var a = new Recurso("A", log))         // 'using' com bloco: libera ao sair do bloco
        {
            a.Usar();
        }
        log.Add("saiu do bloco using");

        ComUsingDeclaration(log);
        log.Add("voltou ao endpoint");

        try
        {
            using var c = new Recurso("C", log);
            throw new InvalidOperationException("erro no meio");
        }
        catch (InvalidOperationException)
        {
            log.Add("erro capturado");
        }

        return Ok(log);
    }


    /// <summary>Destrutor (finalizer) x Dispose; GC.SuppressFinalize.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CriarSemDispose(int quantidade)
    {
        for (int i = 0; i < quantidade; i++)
        {
            var r = new ComFinalizer();
            GC.KeepAlive(r);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CriarComDispose(int quantidade)
    {
        for (int i = 0; i < quantidade; i++)
        {
            using var r = new ComFinalizer();
        }
    }

    [HttpGet("dispose-vs-finalizer")]
    public IActionResult DisposeVsFinalizer()
    {
        int antes = ComFinalizer.Esquecidos;

        CriarSemDispose(10);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        int semDispose = ComFinalizer.Esquecidos - antes;

        antes = ComFinalizer.Esquecidos;
        CriarComDispose(10);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        int comDispose = ComFinalizer.Esquecidos - antes;

        return Ok(new { finalizadosSemDispose = semDispose, finalizadosComDispose = comDispose });
    }

    /// <summary>IAsyncDisposable e await using.</summary>
    public class RecursoAsync : IAsyncDisposable
    {
        private readonly List<string> _log;

        public RecursoAsync(List<string> log)
        {
            _log = log;
            _log.Add("aberto");
        }

        public async ValueTask DisposeAsync()
        {
            _log.Add("liberando (assíncrono)...");
            await Task.Delay(50);                     // ex.: descarregar dados pela rede
            _log.Add("liberado (DisposeAsync)");
        }
    }

}
