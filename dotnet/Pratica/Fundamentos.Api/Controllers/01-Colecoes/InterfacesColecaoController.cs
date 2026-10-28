using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>IEnumerable, ICollection, IList, IReadOnlyList: contratos, não implementações.</summary>
[Route("api/colecoes/interfaces")]
[Tags("01.12 - Interfaces de coleção")]
public class InterfacesColecaoController : EstudoControllerBase
{
    /// <summary>Quem herda de quem e o que cada interface oferece.</summary>
    [HttpGet("hierarquia")]
    public IActionResult Hierarquia()
    {
        object[] colecoes =
        {
        new List<int>(),
        new int[0],
        new HashSet<int>(),
        new Queue<int>(),
        new Stack<int>()
    };

        var resultado = colecoes.Select(c => new
        {
            tipo = c.GetType().Name,
            iEnumerable = c is IEnumerable<int>,
            iCollection = c is ICollection<int>,
            iList = c is IList<int>,
            iReadOnlyCollection = c is IReadOnlyCollection<int>,
            iReadOnlyList = c is IReadOnlyList<int>
        });

        return Ok(resultado);
    }


    /// <summary>IReadOnlyCollection x List exposta (como o Galo protege agregados).</summary>
    [HttpGet("somente-leitura")]
    public IActionResult SomenteLeitura()
    {
        var elenco = new Elenco();
        elenco.Contratar("Hulk");

        // elenco.JogadoresSeguro.Add("X");   // não compila: IReadOnlyCollection não tem Add

        bool ataquePeloSeguro = false;
        if (elenco.JogadoresSeguro is List<string> listaSegura)
        {
            listaSegura.Add("Intruso");
            ataquePeloSeguro = true;
        }

        bool ataquePeloVulneravel = false;
        if (elenco.JogadoresVulneravel is List<string> listaVulneravel)
        {
            listaVulneravel.Add("Intruso");
            ataquePeloVulneravel = true;
        }

        return Ok(new { ataquePeloSeguro, ataquePeloVulneravel, elencoFinal = elenco.JogadoresSeguro });
    }


    /// <summary>InvalidOperationException ao alterar a coleção durante o foreach.</summary>
    [HttpGet("modificar-no-foreach")]
    public IActionResult ModificarNoForeach()
    {
        var numeros = new List<int> { 1, 2, 3, 4 };
        string? erro = null;

        try
        {
            foreach (int n in numeros)
            {
                if (n == 2)
                    numeros.Remove(n);          // altera a lista durante o foreach
            }
        }
        catch (InvalidOperationException ex)
        {
            erro = ex.Message;
        }

        // correção 1: remover tudo que atende à condição, de uma vez
        var comRemoveAll = new List<int> { 1, 2, 3, 4 };
        comRemoveAll.RemoveAll(n => n % 2 == 0);

        // correção 2: percorrer uma CÓPIA e alterar o original
        var comCopia = new List<int> { 1, 2, 3, 4 };
        foreach (int n in comCopia.ToList())
        {
            if (n % 2 == 0)
                comCopia.Remove(n);
        }

        return Ok(new { erro, comRemoveAll, comCopia });
    }

}
