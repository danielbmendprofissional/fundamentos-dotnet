using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Matriz T[,] (retangular) x T[][] (jagged).</summary>
[Route("api/colecoes/matrizes")]
[Tags("01.02 - Matrizes")]
public class MatrizesController : EstudoControllerBase
{
    /// <summary>Matriz T[,]: criar e percorrer com GetLength.</summary>
    [HttpGet("retangular")]
    public IActionResult Retangular()
    {
        int[,] matriz =
        {
        { 1, 2, 3 },     // linha 0
        { 4, 5, 6 }      // linha 1
    };

        int linhas = matriz.GetLength(0);    // dimensão 0 = linhas
        int colunas = matriz.GetLength(1);   // dimensão 1 = colunas

        // percorrer: um for dentro do outro (linhas por fora, colunas por dentro)
        var percorrida = new List<int[]>();
        for (int i = 0; i < linhas; i++)
        {
            var linha = new int[colunas];
            for (int j = 0; j < colunas; j++)
                linha[j] = matriz[i, j];
            percorrida.Add(linha);
        }

        return Ok(new
        {
            linhas,
            colunas,
            totalDeItens = matriz.Length,        // Length conta TODOS os itens
            itemLinha1Coluna2 = matriz[1, 2],
            matriz = percorrida
        });
    }


    /// <summary>Array de arrays com linhas de tamanhos diferentes.</summary>
    [HttpGet("jagged")]
    public IActionResult Jagged()
    {
        int[][] jagged = new int[3][];      // 3 linhas, ainda sem conteúdo (null)

        jagged[0] = new[] { 1 };            // linha com 1 item
        jagged[1] = new[] { 2, 3 };         // linha com 2 itens
        jagged[2] = new[] { 4, 5, 6 };      // linha com 3 itens

        return Ok(new
        {
            jagged,
            tamanhosDasLinhas = jagged.Select(l => l.Length),
            itemLinha2Coluna1 = jagged[2][1]     // dois colchetes, um por vez
        });
    }


    /// <summary>Somar a diagonal principal de uma matriz quadrada.</summary>
    [HttpGet("diagonal")]
    public IActionResult Diagonal()
    {
        int[,] m =
        {
        { 1, 2,  3 },
        { 4, 5,  6 },
        { 7, 8, 10 }
    };

        int n = m.GetLength(0);              // matriz QUADRADA: linhas == colunas
        int principal = 0;
        int secundaria = 0;

        for (int i = 0; i < n; i++)
        {
            principal += m[i, i];            // (0,0) (1,1) (2,2)
            secundaria += m[i, n - 1 - i];   // (0,2) (1,1) (2,0)
        }

        return Ok(new { principal, secundaria });
    }


    /// <summary>Tabuleiro da velha 3x3.</summary>
    [HttpGet("tabuleiro")]
    public IActionResult Tabuleiro()
    {
        char[,] t = new char[3, 3];

        for (int i = 0; i < 3; i++)          // começa tudo vazio ('.')
            for (int j = 0; j < 3; j++)
                t[i, j] = '.';

        t[0, 0] = 'X';
        t[1, 1] = 'X';
        t[2, 2] = 'X';
        t[0, 2] = 'O';
        t[1, 0] = 'O';

        bool xGanhouNaDiagonal = true;
        for (int i = 0; i < 3; i++)
            if (t[i, i] != 'X')
                xGanhouNaDiagonal = false;

        var linhas = new List<string>();     // monta cada linha como texto, só para exibir
        for (int i = 0; i < 3; i++)
        {
            var chars = new char[3];
            for (int j = 0; j < 3; j++)
                chars[j] = t[i, j];
            linhas.Add(new string(chars));
        }

        return Ok(new { tabuleiro = linhas, xGanhouNaDiagonal });
    }

}
