using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fundamentos.Api.Controllers;

/// <summary>HashSet&lt;T&gt;: conjunto sem duplicados.</summary>
[Route("api/colecoes/hashsets")]
[Tags("01.05 - HashSets")]
public class HashSetsController : EstudoControllerBase
{
    /// <summary>Remover e detectar duplicados.</summary>
    [HttpGet("duplicados")]
    public IActionResult Duplicados()
    {
        var nomes = new List<string> { "Hulk", "Scarpa", "Hulk", "Rony", "Scarpa", "Hulk" };

        // 1) remover duplicados: basta jogar a lista num HashSet
        var unicos = new HashSet<string>(nomes);

        // 2) Add devolve bool: true se entrou, false se já existia
        bool entrouPaulinho = unicos.Add("Paulinho");
        bool entrouHulkDeNovo = unicos.Add("Hulk");

        // 3) detectar quem repetiu: o Add que devolve false denuncia o repetido
        var vistos = new HashSet<string>();
        var repetidos = new HashSet<string>();
        foreach (string nome in nomes)
        {
            if (!vistos.Add(nome))
                repetidos.Add(nome);
        }

        return Ok(new { original = nomes, unicos, entrouPaulinho, entrouHulkDeNovo, repetidos });
    }


    /// <summary>UnionWith, IntersectWith, ExceptWith.</summary>
    [HttpGet("operacoes-de-conjunto")]
    public IActionResult OperacoesDeConjunto()
    {
        var atacantes = new HashSet<string> { "Hulk", "Paulinho", "Rony" };
        var titulares = new HashSet<string> { "Everson", "Hulk", "Scarpa", "Rony" };

        // as operações ...With ALTERAM o conjunto, então trabalhamos em cópias
        var uniao = new HashSet<string>(atacantes);
        uniao.UnionWith(titulares);               // todos, sem repetir

        var interseccao = new HashSet<string>(atacantes);
        interseccao.IntersectWith(titulares);     // só quem está nos dois

        var diferenca = new HashSet<string>(atacantes);
        diferenca.ExceptWith(titulares);          // está em atacantes e NÃO em titulares

        return Ok(new
        {
            atacantes,
            titulares,
            uniao,
            interseccao,
            diferenca,
            temAlgoEmComum = atacantes.Overlaps(titulares)
        });
    }


    /// <summary>Medir Contains em List x HashSet com 100 mil itens (Stopwatch).</summary>
    [HttpGet("list-vs-hashset")]
    public IActionResult ListVsHashSet()
    {
        // Talvez a única situação do meu contexto de desenvolvimento que presta
        // Tenho mil recursos e só quero saber se possui ou não
        // Hash é bem mais rapido pois é O(1) lista vai percorrer tudo O(n)

        const int total = 100_000;
        const int repeticoes = 1_000;

        var lista = Enumerable.Range(0, total).ToList();
        var conjunto = lista.ToHashSet();
        int procurado = total - 1;               // o ÚLTIMO item: pior caso para a lista

        bool achou = true;

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < repeticoes; i++)
            achou &= lista.Contains(procurado);
        sw.Stop();
        double msLista = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < repeticoes; i++)
            achou &= conjunto.Contains(procurado);
        sw.Stop();
        double msHashSet = sw.Elapsed.TotalMilliseconds;

        return Ok(new { total, repeticoes, achou, msLista, msHashSet });
    }

}
