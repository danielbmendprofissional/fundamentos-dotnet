// =============================================== BAD ===============================================
using DesignPatterns.Facade.Solution;
using DesignPatterns.Facade.Solution.Facade;
using DesignPatterns.Facade.Solution.Service;
using Microsoft.Extensions.DependencyInjection;

using pain = DesignPatterns.Facade.Pain;

// Controller needs to know too much... This is a bad thing
// much trash
var services = new ServiceCollection();

services.AddTransient<pain.IEstoque, pain.EstoqueService>();
services.AddTransient<pain.IPagamento, pain.PagamentoService>();
services.AddTransient<pain.IFiscal, pain.FiscalService>();
services.AddTransient<pain.ILogistica, pain.LogisticaService>();

services.AddTransient<pain.OrdemCompraService>();

var provider = services.BuildServiceProvider();
var serviceRuim = provider.GetRequiredService<pain.OrdemCompraService>();

serviceRuim.FinalizarVenda("Playstation 5", 4500.00m);


// =============================================== GOOD ===============================================

var services2 = new ServiceCollection();

// 1. The Console only says: "I want the sales module"
services2.AddVendaModule();
var provider2 = services2.BuildServiceProvider();

// --- EXECUTION ---

// 2. The Console asks only for the Facade
var venda = provider2.GetRequiredService<IOrdemCompraService>();

// 3. A single call resolves everything
venda.ExecutarCompraFluxoSimples("Notebook Gamer", 8000.00m);