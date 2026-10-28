using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bad = DesignPatterns.TemplateMethod.Bad;
using Gof = DesignPatterns.TemplateMethod.Good.GOF;
using Micro = DesignPatterns.TemplateMethod.Good.Microsoft;

// =============================================== BAD ===============================================

Console.WriteLine("--- [BAD] Fluxo Duplicado ---");
new Bad.PagamentoBoletoBad().Processar();
new Bad.PagamentoCartaoBad().Processar();

// =============================================== GOOD ===============================================

// ============================= GOF
Console.WriteLine("\n--- [GOOD - GOF] Herança Clássica (Abstract Class) ---");
Gof.ProcessadorPagamento pagamentoGof = new Gof.PagamentoBoleto();
pagamentoGof.TemplateMethod();

// ============================= MICROSOFT
Console.WriteLine("\n--- [GOOD - MICROSOFT] Injeção de Dependência e Delegates ---");

// 1. Setup DI (Ambiente Profissional)
var services = new ServiceCollection()
    .AddLogging(opt => opt.AddConsole())
    .AddTransient<Micro.ProcessadorPagamentoMicrosoft>()
    .BuildServiceProvider();

var processador = services.GetRequiredService<Micro.ProcessadorPagamentoMicrosoft>();

// 2. Execução: O "Template" é fixo, o comportamento é passado via Lambda
// Isso é muito mais flexível que criar uma classe para cada novo tipo de pagamento.
await processador.ProcessarAsync("Boleto Bancário", async () => {
    Console.WriteLine("[Lambda] Gerando linha digitável...");
    await Task.Delay(100);
});

await processador.ProcessarAsync("Cartão de Crédito", async () => {
    Console.WriteLine("[Lambda] Comunicando com Gateway de Pagamento...");
    await Task.Delay(100);
});

// ===================================================================================================
// CONCLUSÃO: 
// O GOF utiliza Herança (is-a). O Microsoft Way utiliza Composição/Delegates (has-a).
// A versão Microsoft é a mais utilizada no mercado pois permite mudar o comportamento
// em tempo de execução sem criar novas classes e facilita absurdamente os Testes Unitários.
// ===================================================================================================