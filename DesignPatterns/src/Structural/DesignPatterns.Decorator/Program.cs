using DesignPatterns.Decorator;
using DesignPatterns.Decorator._2;
using DesignPatterns.Decorator._2.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDecoratorServices();
builder.Services.AddPedidoServices();

using IHost host = builder.Build();

// ==================================================== EXAMPLE 1 ====================================================

Console.WriteLine("=== EXEMPLO 1: WEATHER SERVICE COM RETRY ===");
var weatherService = host.Services.GetRequiredService<IWeatherService>();

try
{
    var result = weatherService.GetWeather();
    Console.WriteLine($"Resultado Final: {result}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}\n");
}

// ==================================================== EXAMPLE 2 ====================================================

Console.WriteLine("=== EXEMPLO 2: PEDIDO COM BLACK FRIDAY ===");
var meuPedido = host.Services.GetRequiredService<IPedido>();

Console.WriteLine($"Itens: {meuPedido.GetDescricao()}");
Console.WriteLine($"Total a pagar: {meuPedido.CalcularTotal():C}");