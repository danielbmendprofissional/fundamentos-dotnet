//using Bad = DesignPatterns.Strategy.Bad;
using Gof = DesignPatterns.Interpreter.Good.GOF;
using Micro = DesignPatterns.Interpreter.Good.Microsoft;

// =============================================== BAD ===============================================

// No examples needed...

// =============================================== GOOD ===============================================

// ============================= GOF

Console.WriteLine("--- [GOOD - GOF] Interpreter Manual ---");
var contexto = new Gof.Contexto { ValorReferencia = 150 };
// Regra: (Valor > 100) AND (Valor > 50)
var regraGof = new Gof.AndExpressao(new Gof.NumeroExpressao(100), new Gof.NumeroExpressao(50));
Console.WriteLine($"Resultado GOF: {regraGof.Interpretar(contexto)}");

// ============================= MICROSOFT

Console.WriteLine("\n--- [GOOD - MICROSOFT] Expression Trees ---");
var motor = new Micro.EvaluatorMicrosoft();
var regraCompilada = motor.CriarRegraMaiorQue(100);
Console.WriteLine($"Resultado Microsoft (150 > 100): {regraCompilada(150)}");

// ===================================================================================================

// In .NET just use LINQ expressions...