using Microsoft.AspNetCore.Mvc;

namespace Fundamentos.Api.Controllers;

/// <summary>Array T[]: tamanho fixo, memória contígua, acesso por índice.</summary>
[Route("api/colecoes/arrays")]
[Tags("01.01 - Arrays")]
public class ArraysController : EstudoControllerBase
{
    /// <summary>Declarar, inicializar e ler um array.</summary>
    [HttpGet("criar")]
    public IActionResult Criar()
    {

        //Por isso objetos de dominio não usam array, porque não da pra aumentar com mais dados, add, etc...

        //reservando tamanho
        int[] numeros = new int[3];
        numeros[0] = 1;
        numeros[1] = 2;
        numeros[2] = 3;

        //array de 3 vazio
        int[] vazio = new int[3];

        //criando ja com os valores fixando o tamanho
        string[] times = { "Atlético", "Cruzeiro", "América" };

        return Ok(new
        {
            vazio,
            numeros,
            tamanho = numeros.Length,
            segundoItem = numeros[1],
            times
        });
    }

    /// <summary>Provocar IndexOutOfRangeException de propósito e ler o erro.</summary>
    [HttpGet("fora-do-limite")]
    public IActionResult ForaDoLimite()
    {

        //Aqui só corrobora com a ideia de que um array não muda tamanho, veja tentando adicionar um valor em uma quarta posição em um array de 3

        int[] numeros = new int[3];          // índices válidos: 0, 1 e 2

        try
        {
            numeros[3] = 40;                 // a posição 3 não existe
            return Ok("não chegou aqui");
        }
        catch (IndexOutOfRangeException ex)
        {
            return Ok(new
            {
                tipo = ex.GetType().Name,
                mensagem = ex.Message,
                tamanho = numeros.Length,
                ultimoIndiceValido = numeros.Length - 1
            });
        }
    }


    /// <summary>Array.Resize: o que acontece de verdade? Cria outro array.</summary>
    [HttpGet("redimensionar")]
    public IActionResult Redimensionar()
    {
        //A única opção de mudar o tamanho e aumentar um array é um "resize" que nada mais é que criar um novo array com os valores do atual + os novos espaços

        int[] original = { 1, 2, 3 };
        int[] referenciaAntiga = original;   // outra variável apontando para o MESMO array

        Array.Resize(ref original, 5);       // cria um array de 5, copia 1, 2, 3 e troca a variável

        return Ok(new
        {
            original,                                                  // agora é outro array
            referenciaAntiga,                                          // continua o array antigo
            mesmoArray = ReferenceEquals(original, referenciaAntiga)
        });
    }


    /// <summary>Array.Sort e Array.BinarySearch.</summary>
    [HttpGet("ordenar-e-buscar")]
    public IActionResult OrdenarEBuscar()
    {
        //da pra ordenar, listar e buscar

        int[] numeros = { 42, 7, 19, 3, 25 };
        int[] antes = (int[])numeros.Clone();      // cópia para mostrar o "antes"

        Array.Sort(numeros);                       // ordena o próprio array

        int achou = Array.BinarySearch(numeros, 19);     // existe: devolve a posição
        int naoAchou = Array.BinarySearch(numeros, 20);  // não existe: devolve negativo

        return Ok(new { antes, depois = numeros, achou, naoAchou });
    }

}
