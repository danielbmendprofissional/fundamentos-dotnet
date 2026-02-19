using System.Diagnostics;

var acao1 = new Action(Processo1);
var acao2 = new Action(Processo2);
var acao3 = new Action(Processo3);

var stopWatch = new Stopwatch();
//Processo executado por multiplas threads em paralelo
stopWatch.Start();
Parallel.Invoke(acao1, acao2, acao3);
stopWatch.Stop();
Console.WriteLine($"O tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");

void Processo1()
{
    Console.WriteLine($"Processo 1 finalizado Thread: {Thread.CurrentThread.ManagedThreadId}");
    //Exemplo da parte de um processo demorado, thread demorando 2 segundos para realizar esse processo.
    Thread.Sleep(2000);
}

void Processo2()
{
    Console.WriteLine($"Processo 2 finalizado Thread: {Thread.CurrentThread.ManagedThreadId}");
    //Exemplo da parte de um processo demorado, thread demorando 2 segundos para realizar esse processo.
    Thread.Sleep(2000);
}

void Processo3()
{
    Console.WriteLine($"Processo 3 finalizado Thread: {Thread.CurrentThread.ManagedThreadId}");
    //Exemplo da parte de um processo demorado, thread demorando 2 segundos para realizar esse processo.
    Thread.Sleep(2000);
}