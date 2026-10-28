using Pure = DesignPatterns.Factory.Solution.pure;
using PureProduct = DesignPatterns.Factory.Solution.pure.Products;
using PureFactories = DesignPatterns.Factory.Solution.pure.Factories;
using EnumWay = DesignPatterns.Factory.Solution.enumWay;
using DesignPatterns.Factory.Solution.Microsoft;
using DesignPatterns.Factory.Solution.Microsoft.Factory;
using DesignPatterns.Factory.Solution.Microsoft.Products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// =========================================================================
// MICROSOFT  
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKeyedTransient<IPagamento, Pix>(TipoPagamento.Pix);
builder.Services.AddKeyedTransient<IPagamento, Boleto>(TipoPagamento.Boleto);

builder.Services.AddTransient<IPagamentoFactory, PagamentoFactory>();

using IHost host = builder.Build();

// =================================================   PURE   ======================================================

Console.WriteLine("=== CHECKOUT SYSTEM ===");
Console.WriteLine("Choose method: 1-Pix | 2-Credit Card");
var escolha = Console.ReadLine();

// In an API we could use Dependency Injection to resolve the factory
// or just use endpoint routing to different factories based on the route called.
// Here we are simulating user choice at runtime...

Pure.PagamentoCreator meuGateway = escolha switch
{
    "1" => new PureFactories.PixFactory(),
    "2" => new PureFactories.CreditoFactory(),
    _ => throw new Exception("Invalid option!")
};

ProcessarPedido(meuGateway, 150.50m);

void ProcessarPedido(Pure.PagamentoCreator gateway, decimal total)
{
    Console.WriteLine("Starting order processing...");

    // They don't know which factory they are using, but they know it follows the same interface and works.
    gateway.FinalizarVenda(total);

    Console.WriteLine("Order completed successfully!");
}

// =================================================   ENUM   ======================================================

// In .NET we see a lot of factories implemented using Enums.
// But depending on the scenario it is a bad practice, because every time you add a new product
// you need to modify the factory code, violating OCP (Open Closed Principle),
// but it is less code.

Console.WriteLine("--- QUICK CHECKOUT ---");
Console.WriteLine("Choose: 1-Credit | 2-Debit | 3-Pix | 4-Boleto");
var entrada = Console.ReadLine();
var entradaInt = int.Parse(entrada);

PureProduct.IPagamento pagamento = EnumWay.PagamentoFactory.Create((EnumWay.MetodoPagamento)entradaInt);

pagamento.Processar(250.00m);