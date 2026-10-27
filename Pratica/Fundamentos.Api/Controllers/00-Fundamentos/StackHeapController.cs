using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fundamentos.Api.Controllers;

public struct Ponto { public int X; public int Y; }
public class Pessoa { public int Idade; public long Id; }

/// <summary>Onde as coisas vivem na memória: pilha (stack) x monte (heap).</summary>
[Route("api/fundamentos/stack-heap")]
[Tags("00.02 - Stack e Heap")]
public class StackHeapController : EstudoControllerBase
{
    /// <summary>Tipo valor local na stack; objeto no heap; a referência na stack apontando para o heap.</summary>
    [HttpGet("onde-vive-cada-variavel")]
    public IActionResult OndeViveCadaVariavel()
    {
        int idade = 30;                        // valor direto na STACK
        int copiaIdade = idade;                // copia o VALOR: agora são dois independentes
        copiaIdade = 99;                       // não afeta 'idade'

        var lista = new List<int> { 1, 2 };    // 'lista' (o endereço) na STACK; o objeto List no HEAP
        var outraReferencia = lista;           // copia só o ENDEREÇO: os dois apontam para o mesmo objeto
        outraReferencia.Add(3);                // mexe no objeto que os dois enxergam

        return Ok(new
        {
            idade,
            copiaIdade,
            lista,
            outraReferencia,
            mesmoObjeto = ReferenceEquals(lista, outraReferencia)
        });
    }


    /// <summary>Cada chamada empilha um frame: ver com StackTrace (ligação com Stack&lt;T&gt;).</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static List<string> Passo3() =>
        new StackTrace().GetFrames()
            .Take(4)
            .Select(f => f.GetMethod()!.Name)
            .ToList();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static List<string> Passo2() => Passo3();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static List<string> Passo1() => Passo2();

    [HttpGet("pilha-de-chamadas")]
    public IActionResult PilhaDeChamadas() => Ok(Passo1());

    /// <summary>Recursão controlada: profundidade x tamanho da stack. StackOverflow não é capturável e derruba o processo.</summary>
    private static long SomarAte(int n) => n == 0 ? 0 : n + SomarAte(n - 1);

    [HttpGet("recursao-com-limite")]
    public IActionResult RecursaoComLimite([FromQuery] int profundidade = 1000)
    {
        const int limite = 2_000;

        if (profundidade > limite)
            return BadRequest(new { mensagem = $"limite de {limite} níveis, para não estourar a stack" });

        return Ok(new { profundidade, soma = SomarAte(profundidade) });
    }

    /// <summary>Quanto cada tipo ocupa: int, long, struct e referência.</summary>
    [HttpGet("tamanho-dos-tipos")]
    public IActionResult TamanhoDosTipos() => Ok(new Dictionary<string, int>
    {
        ["byte"] = Unsafe.SizeOf<byte>(),
        ["bool"] = Unsafe.SizeOf<bool>(),
        ["char"] = Unsafe.SizeOf<char>(),
        ["int"] = Unsafe.SizeOf<int>(),
        ["long"] = Unsafe.SizeOf<long>(),
        ["double"] = Unsafe.SizeOf<double>(),
        ["decimal"] = Unsafe.SizeOf<decimal>(),
        ["Guid"] = Unsafe.SizeOf<Guid>(),
        ["struct Ponto (int, int)"] = Unsafe.SizeOf<Ponto>(),
        ["class Pessoa (a referência)"] = Unsafe.SizeOf<Pessoa>()
    });

    /// <summary>Memória na stack com stackalloc e Span&lt;T&gt;.</summary>
    [HttpGet("stackalloc")]
    public IActionResult Stackalloc()
    {
        Span<int> buffer = stackalloc int[5];       // 5 ints reservados NA STACK

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = i * i;

        int soma = 0;
        foreach (int n in buffer)
            soma += n;

        return Ok(new { valores = buffer.ToArray(), soma });
    }

}
