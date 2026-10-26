using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Dictionary&lt;TKey,TValue&gt;: tabela hash, busca por chave.</summary>
[Route("api/colecoes/dicionarios")]
[Tags("01.04 - Dicionários")]
public class DicionariosController : EstudoControllerBase
{

    public class CamisaClasse
    {
        public int Numero { get; }
        public CamisaClasse(int numero) => Numero = numero;
    }

    public record CamisaRecord(int Numero);


    /// <summary>Indexador x TryGetValue (KeyNotFoundException).</summary>
    [HttpGet("buscar")]
    public IActionResult Buscar([FromQuery] int camisa = 99)
    {
        var jogadores = new Dictionary<int, string>
        {
            [9] = "Hulk",
            [10] = "Scarpa",
            [7] = "Paulinho"
        };

        bool achou = jogadores.TryGetValue(camisa, out string? nome);   // não lança exceção

        string? erroDoIndexador = null;
        try
        {
            _ = jogadores[camisa];                                      // lança se não existir
        }
        catch (KeyNotFoundException ex)
        {
            erroDoIndexador = ex.Message;
        }

        return Ok(new { camisa, achou, nome, erroDoIndexador });
    }


    /// <summary>Add x indexador x TryAdd (chave duplicada).</summary>
    [HttpPost("adicionar")]
    public IActionResult Adicionar()
    {
        var d = new Dictionary<string, int>();

        d.Add("Hulk", 9);                    // cria
        d["Scarpa"] = 10;                    // indexador: cria se não existe
        d["Hulk"] = 99;                      // indexador: SOBRESCREVE se existe

        bool tryAddNovo = d.TryAdd("Rony", 11);      // true: não existia
        bool tryAddExistente = d.TryAdd("Hulk", 1);  // false: já existe, NÃO altera

        string? erroDoAdd = null;
        try
        {
            d.Add("Hulk", 5);                // Add com chave repetida lança exceção
        }
        catch (ArgumentException ex)
        {
            erroDoAdd = ex.Message;
        }

        return Ok(new { dicionario = d, tryAddNovo, tryAddExistente, erroDoAdd });
    }


    /// <summary>Contar a frequência de palavras de um texto.</summary>
    [HttpGet("frequencia-palavras")]
    public IActionResult FrequenciaPalavras()
    {
        string texto = "galo é galo e galo é massa";
        var frequencia = new Dictionary<string, int>();

        foreach (string palavra in texto.Split(' '))
        {
            frequencia[palavra] = frequencia.GetValueOrDefault(palavra) + 1;   // 0 se ainda não existe
        }

        return Ok(frequencia);
    }


    /// <summary>Equals e GetHashCode: usar um Value Object como chave.</summary>
    [HttpGet("chave-customizada")]
    public IActionResult ChaveCustomizada()
    {
        //Nesse caso vai dar erro porque toda NEW classe é um novo objeto, novo hash, então a key é diferente
        var porClasse = new Dictionary<CamisaClasse, string>();
        porClasse[new CamisaClasse(9)] = "Hulk";
        bool achouComClasse = porClasse.ContainsKey(new CamisaClasse(9));

        //Nesse caso vai dar certo porque record, VOs comparam por valor e não por recurso novo, ele só verifica o record = 9, então a key é igual
        var porRecord = new Dictionary<CamisaRecord, string>();
        porRecord[new CamisaRecord(9)] = "Hulk";
        bool achouComRecord = porRecord.ContainsKey(new CamisaRecord(9));

        return Ok(new { achouComClasse, achouComRecord });
    }

}
