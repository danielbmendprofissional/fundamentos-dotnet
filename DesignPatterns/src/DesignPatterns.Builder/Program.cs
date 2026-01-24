using DesignPatterns.Builder.Solution.Builder;
using DesignPatterns.Builder.Solution.Classes;
using DesignPatterns.Builder.Solution.Director;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IProdutoComplexoBuilder, ProdutoComplexoBuilder>();
builder.Services.AddScoped<IProdutoComplexoDiretor, IProdutoComplexoDiretor>();

using IHost host = builder.Build();

var services = new ServiceCollection();
var serviceProvider = services.BuildServiceProvider();
//=======================================================   PAIN   ======================================================

// Client needs to now how to construct a complex object step by step
var produto = new ProdutoComplexoAnemico();

// Inconvenience 1: The client needs to know what properties to set
produto.Id = 1;
produto.Nome = "Servidor Dell PowerEdge";
produto.Peso = 12.0;

// Inconvenience 2: The client needs to know valid combinations of properties
// What if a Servidor must have at least 2 items related?
produto.ItensRelacionados = new List<ItemPedidoAnemico>();

var item1 = new ItemPedidoAnemico { Nome = "Memória 16GB", Preco = 400m };
var item2 = new ItemPedidoAnemico { Nome = "HD 1TB", Preco = 300m };

produto.ItensRelacionados.Add(item1);
produto.ItensRelacionados.Add(item2);

// Inconvenience 3: The client needs to know the order of setting properties
produto.Fornecedor = new FornecedorAnemico
{
    NomeFantasia = "Dell Brasil",
    CNPJ = "00.111.222/0001-33"
};

// Inconvenience 4: The client is responsible for creating and assembling the product

Console.WriteLine($"Criado: {produto.Nome}");

//======================================================= SOLUTION ======================================================
// --- EXECUÇÃO ---

// Não conseguimos dar 'new ProdutoComplexoDirector()', então pedimos ao DI
var diretor = serviceProvider.GetRequiredService<IProdutoComplexoDiretor>();
var productBuilder = serviceProvider.GetRequiredService<ProdutoComplexoBuilder>();

diretor.ConstruirServidorDellPadrao();
var produto = productBuilder.Build();