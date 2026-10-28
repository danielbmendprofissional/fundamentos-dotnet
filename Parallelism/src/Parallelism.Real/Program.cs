using Parallelism.Real;
using Parallelism.Real.Models;
using System.Diagnostics;
using System.Threading.Tasks;

string[] ceps = new string[5];
ceps[0] = "07155081";
ceps[1] = "15800100";
ceps[2] = "38407369";
ceps[3] = "77445100";
ceps[4] = "78015818";

var listaCep = new List<CepModel>();

//Aqui estou informando qual o máximo de threads que o paralelismo vai utilizar.
var parallelOptions = new ParallelOptions();
parallelOptions.MaxDegreeOfParallelism = 2;

var stopWatch = new Stopwatch();
stopWatch.Start();
Parallel.ForEach(ceps, parallelOptions, cep =>
{
    listaCep.Add(new ViaCepService().GetCep(cep));
});


//Exemplo onde tudo será processado na mesma Thread, cada um dos ceps será buscado na mesma thread 1 por vez.

// foreach(var cep in ceps)
// {
//     listaCep.Add(new ViaCepService().GetCep(cep));
// }
stopWatch.Stop();
Console.WriteLine($"O tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");

listaCep
    .OrderBy(x => x.Bairro)
    .ToList()
    .ForEach(cep => Console.WriteLine(cep));

//OBSERVAÇÕES

/*
    - Paralelismo não executa de forma sequencial, ele vai executando todos nas threads disponíveis e vai retornando
    a medida que for finalizando, sem ordenação. Assim caso precise de executar dados de forma sequencial, não é re-
    comendado utiliza-lo.
*/
