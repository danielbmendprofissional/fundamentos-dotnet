using DesignPatterns.Builder.Solution.Builder;
using DesignPatterns.Builder.Solution.Classes;
using DesignPatterns.Builder.Solution.Director;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IProdutoComplexoBuilder, ProdutoComplexoBuilder>();
builder.Services.AddScoped<IProdutoComplexoDiretor, ProdutoComplexoDiretor>();

using IHost host = builder.Build();

//=======================================================   PAIN   ======================================================

var produto = new ProdutoComplexoAnemico();

produto.Id = 1;
produto.Nome = "Servidor Dell PowerEdge";
produto.Peso = 12.0;

produto.ItensRelacionados = new List<ItemPedidoAnemico>();

var item1 = new ItemPedidoAnemico { Nome = "Memória 16GB", Preco = 400m };
var item2 = new ItemPedidoAnemico { Nome = "HD 1TB", Preco = 300m };

produto.ItensRelacionados.Add(item1);
produto.ItensRelacionados.Add(item2);

produto.Fornecedor = new FornecedorAnemico
{
    NomeFantasia = "Dell Brasil",
    CNPJ = "00.111.222/0001-33"
};

Console.WriteLine($"Created manually: {produto.Nome}");

//======================================================= SOLUTION ======================================================

var diretor = host.Services.GetRequiredService<IProdutoComplexoDiretor>();
var productBuilder = host.Services.GetRequiredService<ProdutoComplexoBuilder>();

diretor.ConstruirServidorDellPadrao();
var produtoSolution = productBuilder.Build();

Console.WriteLine($"Created via Builder/Director: {produtoSolution.Nome}");