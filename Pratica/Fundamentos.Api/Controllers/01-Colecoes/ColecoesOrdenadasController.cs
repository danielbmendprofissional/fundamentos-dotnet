using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fundamentos.Api.Controllers;

/// <summary>SortedDictionary, SortedSet, SortedList: ordem mantida a cada inserção.</summary>
[Route("api/colecoes/ordenadas")]
[Tags("01.09 - Coleções ordenadas")]
public class ColecoesOrdenadasController : EstudoControllerBase
{
    /// <summary>Chaves sempre ordenadas.</summary>
    [HttpGet("sorted-dictionary")]
    public IActionResult SortedDictionary()
    {
        var normal = new Dictionary<string, int>
        {
            ["Hulk"] = 20,
            ["Scarpa"] = 12,
            ["Paulinho"] = 25,
            ["Rony"] = 15
        };

        var ordenado = new SortedDictionary<string, int>
        {
            ["Hulk"] = 20,
            ["Scarpa"] = 12,
            ["Paulinho"] = 25,
            ["Rony"] = 15
        };

        return Ok(new
        {
            normal,
            ordenado,
            primeiraChave = ordenado.Keys.First()
        });
    }


    /// <summary>Conjunto ordenado, GetViewBetween.</summary>
    [HttpGet("sorted-set")]
    public IActionResult SortedSet()
    {
        var numeros = new SortedSet<int> { 42, 7, 19, 3, 25, 7 };   // o 7 repetido é ignorado

        bool adicionouRepetido = numeros.Add(7);                    // false, como no HashSet

        return Ok(new
        {
            numeros,
            adicionouRepetido,
            menor = numeros.Min,
            maior = numeros.Max,
            entre10e30 = numeros.GetViewBetween(10, 30),            // limites inclusivos
            decrescente = numeros.Reverse()
        });
    }


    /// <summary>Comparar o custo das duas estratégias.</summary>
    [HttpGet("ordenar-no-fim-vs-sempre-ordenado")]
    public IActionResult OrdenarNoFimVsSempreOrdenado()
    {
        const int total = 100_000;
        const int pequeno = 2_000;
        var rnd = new Random(42);                                    // semente fixa: mesmos dados sempre
        int[] dados = Enumerable.Range(0, total).Select(_ => rnd.Next()).ToArray();
        long soma = 0;

        // CENÁRIO 1: insere tudo e só precisa da ordem no final
        var sw = Stopwatch.StartNew();
        var lista = new List<int>();
        foreach (int d in dados) lista.Add(d);
        lista.Sort();                                                // ordena UMA vez
        sw.Stop();
        double msListaOrdenaNoFim = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        var conjunto = new SortedSet<int>();
        foreach (int d in dados) conjunto.Add(d);                    // ordenado a cada Add
        sw.Stop();
        double msSortedSetInsere = sw.Elapsed.TotalMilliseconds;

        // CENÁRIO 2: a cada inserção, preciso do MENOR valor
        sw.Restart();
        var listaViva = new List<int>();
        foreach (int d in dados.Take(pequeno))
        {
            listaViva.Add(d);
            listaViva.Sort();                                        // reordena a cada vez
            soma += listaViva[0];
        }
        sw.Stop();
        double msListaReordenando = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        var setVivo = new SortedSet<int>();
        foreach (int d in dados.Take(pequeno))
        {
            setVivo.Add(d);
            soma += setVivo.Min;                                     // já está ordenado
        }
        sw.Stop();
        double msSortedSetConsultando = sw.Elapsed.TotalMilliseconds;

        return Ok(new
        {
            cenario1 = new { total, msListaOrdenaNoFim, msSortedSetInsere },
            cenario2 = new { itens = pequeno, msListaReordenando, msSortedSetConsultando }
        });
    }

    }
