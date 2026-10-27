using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

public record Jogador(string Nome, string Posicao, int Gols, int Idade);

/// <summary>LINQ sobre coleções.</summary>
[Route("api/colecoes/linq")]
[Tags("01.13 - LINQ")]
public class LinqController : EstudoControllerBase
{
    private static readonly List<Jogador> _elenco = new()
    {
        new("Hulk",     "Atacante", 20, 38),
        new("Paulinho", "Atacante", 25, 25),
        new("Rony",     "Atacante", 15, 29),
        new("Scarpa",   "Meia",     12, 31),
        new("Bernard",  "Meia",      5, 33),
        new("Everson",  "Goleiro",   0, 35)
    };

    /// <summary>Where e Select.</summary>
    [HttpGet("filtrar-e-projetar")]
    public IActionResult FiltrarEProjetar()
    {
        var atacantes = _elenco.Where(j => j.Posicao == "Atacante").Select(j => j.Nome);

        var artilheiros = _elenco
            .Where(j => j.Gols >= 15)
            .OrderByDescending(j => j.Gols)
            .Select(j => new { j.Nome, j.Gols });          // projeta só o que interessa

        return Ok(new
        {
            atacantes,
            artilheiros,
            temGoleiro = _elenco.Any(j => j.Posicao == "Goleiro"),
            totalDeGols = _elenco.Sum(j => j.Gols),
            primeiroMeia = _elenco.FirstOrDefault(j => j.Posicao == "Meia")?.Nome,
            maisDe30Anos = _elenco.Count(j => j.Idade > 30)
        });
    }


    /// <summary>GroupBy, ToDictionary, ToLookup.</summary>
    [HttpGet("agrupar")]
    public IActionResult Agrupar()
    {
        var porPosicao = _elenco
            .GroupBy(j => j.Posicao)
            .Select(g => new
            {
                posicao = g.Key,
                quantidade = g.Count(),
                gols = g.Sum(j => j.Gols),
                nomes = g.Select(j => j.Nome)
            });

        var golsPorNome = _elenco.ToDictionary(j => j.Nome, j => j.Gols);

        var porPosicaoLookup = _elenco.ToLookup(j => j.Posicao);

        return Ok(new
        {
            porPosicao,
            golsPorNome,
            atacantesNoLookup = porPosicaoLookup["Atacante"].Select(j => j.Nome),
            zagueirosNoLookup = porPosicaoLookup["Zagueiro"].Count()      // 0, sem exceção
        });
    }


    /// <summary>Deferred execution x ToList (materializar).</summary>
    [HttpGet("execucao-adiada")]
    public IActionResult ExecucaoAdiada()
    {
        //Só executa no .ToList e no Count, se for usar o valor só uma vez roda uma vez o ToList e vai usando o mesmo recurso

        var numeros = new List<int> { 1, 2, 3 };
        int execucoes = 0;

        var consulta = numeros.Where(n => { execucoes++; return n > 1; });
        int aposDefinir = execucoes;                    // 0: nada rodou ainda

        numeros.Add(4);                                 // mudo a origem ANTES de consumir

        var primeira = consulta.ToList();               // roda agora
        int aposPrimeira = execucoes;

        int quantidade = consulta.Count();              // roda DE NOVO
        int aposSegunda = execucoes;

        var foto = numeros.Where(n => n > 1).ToList();  // ToList congela o resultado
        numeros.Add(5);

        return Ok(new
        {
            aposDefinir,
            primeira,
            aposPrimeira,
            quantidade,
            aposSegunda,
            fotoCongelada = foto,
            consultaDepoisDoAdd5 = consulta.ToList()
        });
    }


    private static IEnumerable<int> Pares(int limite, List<string> log)
    {
        for (int i = 1; i <= limite; i++)
        {
            log.Add($"verificando {i}");
            if (i % 2 == 0)
            {
                log.Add($"entregando {i}");
                yield return i;                  // entrega e PAUSA aqui
            }
        }
    }

    [HttpGet("yield")]
    public IActionResult Yield()
    {
        var log = new List<string>();

        var sequencia = Pares(10, log);          // nada executa ainda
        log.Add("sequência criada");

        foreach (int p in sequencia)
        {
            log.Add($"consumi {p}");
            if (p >= 4) break;                   // paro no meio
        }

        return Ok(log);
    }

}
