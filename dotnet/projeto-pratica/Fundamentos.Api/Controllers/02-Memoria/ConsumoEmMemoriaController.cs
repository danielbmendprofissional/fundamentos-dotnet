using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Fundamentos.Api.Controllers;

/// <summary>Quanto dado você traz para a RAM: carregar tudo x processar em fluxo.</summary>
[Route("api/memoria/consumo")]
[Tags("02.03 - Consumo em memória")]
public class ConsumoEmMemoriaController : EstudoControllerBase
{
    private static readonly List<string> _nomes =
        Enumerable.Range(1, 1000).Select(i => $"Jogador {i:D4}").ToList();

    private static double MB(long bytes) => Math.Round(bytes / 1024.0 / 1024.0, 1);

    private static IEnumerable<int> Gerar(int total, int[] produzidos)     // produzidos[0] = contador
    {
        for (int i = 1; i <= total; i++)
        {
            produzidos[0]++;
            yield return i;
        }
    }

    /// <summary>File.ReadAllLines x File.ReadLines: memória e tempo.</summary>
    [HttpGet("carregar-tudo-vs-fluxo")]
    public IActionResult CarregarTudoVsFluxo()
    {
        string caminho = Path.Combine(Path.GetTempPath(), "estudo-consumo.csv");
        System.IO.File.WriteAllLines(caminho, Enumerable.Range(0, 500_000).Select(i => $"{i};Jogador {i};{i % 50}"));

        // A) carrega TUDO na memória
        GC.Collect();
        long baseA = GC.GetTotalMemory(true);
        long alocA = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();

        string[] todas = System.IO.File.ReadAllLines(caminho);
        int contagemA = todas.Count(l => l.EndsWith(";0"));

        double msA = sw.Elapsed.TotalMilliseconds;
        long retidaA = GC.GetTotalMemory(true) - baseA;               // 'todas' ainda está viva aqui
        long alocadaA = GC.GetAllocatedBytesForCurrentThread() - alocA;
        GC.KeepAlive(todas);
        todas = null!;

        // B) UMA linha por vez
        GC.Collect();
        long baseB = GC.GetTotalMemory(true);
        long alocB = GC.GetAllocatedBytesForCurrentThread();
        sw.Restart();

        int contagemB = 0;
        foreach (string linha in System.IO.File.ReadLines(caminho))
        {
            if (linha.EndsWith(";0")) contagemB++;
        }

        double msB = sw.Elapsed.TotalMilliseconds;
        long alocadaB = GC.GetAllocatedBytesForCurrentThread() - alocB;

        // segunda passada, fora da medição de tempo: mede o que fica VIVO durante a leitura
        long picoRetidoB = 0;
        int lidas = 0;
        foreach (string linha in System.IO.File.ReadLines(caminho))
        {
            if (++lidas % 100_000 == 0)
                picoRetidoB = Math.Max(picoRetidoB, GC.GetTotalMemory(true) - baseB);
        }

        System.IO.File.Delete(caminho);

        return Ok(new
        {
            tudo = new { contagem = contagemA, ms = msA, retidoMB = MB(retidaA), alocadoMB = MB(alocadaA) },
            fluxo = new { contagem = contagemB, ms = msB, retidoMB = MB(picoRetidoB), alocadoMB = MB(alocadaB) }
        });
    }

    /// <summary>ToList (tudo na RAM) x IEnumerable com yield (um por vez).</summary>
    [HttpGet("materializar-vs-streaming")]
    public IActionResult MaterializarVsStreaming()
    {
        const int total = 1_000_000;

        // 1) materializa: gera TUDO, guarda TUDO, e só depois pega 5
        var produzidosA = new int[1];
        long antesA = GC.GetAllocatedBytesForCurrentThread();
        var primeirosA = Gerar(total, produzidosA).ToList().Where(n => n % 2 == 0).Take(5).ToList();
        long bytesA = GC.GetAllocatedBytesForCurrentThread() - antesA;

        // 2) streaming: pede um por vez e PARA quando já tem 5
        var produzidosB = new int[1];
        long antesB = GC.GetAllocatedBytesForCurrentThread();
        var primeirosB = Gerar(total, produzidosB).Where(n => n % 2 == 0).Take(5).ToList();
        long bytesB = GC.GetAllocatedBytesForCurrentThread() - antesB;

        return Ok(new
        {
            materializando = new { primeiros = primeirosA, itensProduzidos = produzidosA[0], bytes = bytesA },
            streaming = new { primeiros = primeirosB, itensProduzidos = produzidosB[0], bytes = bytesB }
        });
    }

    /// <summary>Skip e Take: nunca trazer a tabela inteira.</summary>
    [HttpGet("paginacao")]
    public IActionResult Paginacao([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
    {
        tamanho = Math.Clamp(tamanho, 1, 100);            // o cliente nunca pode pedir "tudo"
        pagina = Math.Max(pagina, 1);

        var itens = _nomes
            .OrderBy(n => n)                              // sem ordem fixa, as páginas não são estáveis
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToList();

        int totalDeItens = _nomes.Count;
        int totalDePaginas = (int)Math.Ceiling(totalDeItens / (double)tamanho);

        return Ok(new { pagina, tamanho, totalDeItens, totalDePaginas, itens });
    }

    /// <summary>Onde o filtro roda: no SQL (IQueryable) ou no C# depois do ToList.</summary>
    [HttpGet("iqueryable-vs-ienumerable")]
    public IActionResult IQueryableVsIEnumerable([FromServices] EstudoDbContext db)
    {
        // A) IQueryable: o Where vira SQL, e só as linhas filtradas chegam
        IQueryable<JogadorDb> consulta = db.Jogadores
            .AsNoTracking()
            .Where(j => j.Posicao == "Atacante" && j.Gols >= 25);

        string sql = consulta.ToQueryString().ReplaceLineEndings(" ");

        long antesA = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        var noBanco = consulta.ToList();
        double msA = sw.Elapsed.TotalMilliseconds;
        long bytesA = GC.GetAllocatedBytesForCurrentThread() - antesA;

        // B) ToList() ANTES do Where: traz TUDO e filtra no C#
        long antesB = GC.GetAllocatedBytesForCurrentThread();
        sw.Restart();
        var todos = db.Jogadores.AsNoTracking().ToList();
        var emMemoria = todos.Where(j => j.Posicao == "Atacante" && j.Gols >= 25).ToList();
        double msB = sw.Elapsed.TotalMilliseconds;
        long bytesB = GC.GetAllocatedBytesForCurrentThread() - antesB;

        return Ok(new
        {
            sql,
            filtradoNoBanco = new { linhasTrazidas = noBanco.Count, ms = msA, bytes = bytesA },
            filtradoNoCSharp = new { linhasTrazidas = todos.Count, resultado = emMemoria.Count, ms = msB, bytes = bytesB }
        });
    }

    /// <summary>Por que o provider InMemory do EF engana nos testes (sem constraints, sem transações).</summary>
    [HttpGet("ef-inmemory-vs-banco-real")]
    public IActionResult EfInMemoryVsBancoReal()
    {
        // provider InMemory (uma coleção na memória, sem relacional)
        var opcoesMem = new DbContextOptionsBuilder<EstudoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        string? erroInMemory = null;
        using (var ctx = new EstudoDbContext(opcoesMem))
        {
            ctx.Jogadores.Add(new JogadorDb { Nome = "Hulk", Posicao = "Atacante", Gols = 20 });
            ctx.Jogadores.Add(new JogadorDb { Nome = "Hulk", Posicao = "Atacante", Gols = 21 });   // nome repetido
            try { ctx.SaveChanges(); } catch (Exception ex) { erroInMemory = ex.GetType().Name; }
        }

        // SQLite em memória (banco relacional de verdade)
        using var conexao = new SqliteConnection("DataSource=:memory:");
        conexao.Open();
        var opcoesSqlite = new DbContextOptionsBuilder<EstudoDbContext>().UseSqlite(conexao).Options;

        string? erroSqlite = null;
        using (var ctx = new EstudoDbContext(opcoesSqlite))
        {
            ctx.Database.EnsureCreated();
            ctx.Jogadores.Add(new JogadorDb { Nome = "Hulk", Posicao = "Atacante", Gols = 20 });
            ctx.Jogadores.Add(new JogadorDb { Nome = "Hulk", Posicao = "Atacante", Gols = 21 });
            try { ctx.SaveChanges(); } catch (Exception ex) { erroSqlite = ex.GetType().Name; }
        }

        return Ok(new { erroInMemory, erroSqlite });
    }
}
