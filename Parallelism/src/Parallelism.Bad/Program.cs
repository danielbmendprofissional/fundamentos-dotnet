using System.Diagnostics;

var stopWatch = new Stopwatch();
//Processo executado por 1 Thread
stopWatch.Start();
Processo1();
Processo2();
Processo3();
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