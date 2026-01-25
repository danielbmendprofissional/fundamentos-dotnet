// =============================================== PAIN ===============================================
using DesignPatterns.Facade.Solution;
using DesignPatterns.Facade.Solution.Facade;
using DesignPatterns.Facade.Solution.Service;
using Microsoft.Extensions.DependencyInjection;

using pain = DesignPatterns.Facade.Pain; ;

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


// ============================================== SOLUTION ===============================================

var services2 = new ServiceCollection();

// 1. O Console só diz: "Quero o módulo de venda"
services.AddVendaModule();
var provider2 = services.BuildServiceProvider();

// --- EXECUÇÃO ---

// 2. O Console pede apenas a Fachada
var venda = provider.GetRequiredService<IOrdemCompraService>();

// 3. Uma única chamada resolve tudo
venda.ExecutarCompraFluxoSimples("Notebook Gamer", 8000.00m);