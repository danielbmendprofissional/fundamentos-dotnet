using DesignPatterns.Observer;
using Microsoft.Extensions.DependencyInjection;
using Bad = DesignPatterns.Observer.Bad;
using Gof = DesignPatterns.Observer.Good.GOF;
using Micro = DesignPatterns.Observer.Good.Microsoft;

// =============================================== BAD ===============================================

var estoqueBad = new Bad.Estoque();
Console.WriteLine("--- [BAD] Notificações Hardcoded ---");
estoqueBad.AtualizarEstoque("Processador i9", 3);

// =============================================== GOOD ===============================================

// ============================= GOF

var estoqueGof = new Gof.Estoque();
estoqueGof.Subscribe(new Gof.MarketingObserver());
estoqueGof.Subscribe(new Gof.ComprasObserver());

Console.WriteLine("\n--- [GOOD - GOF] Interfaces Puristas ---");
estoqueGof.AtualizarEstoque("Processador i9", 3);

// ============================= MICROSOFT

// OBRIGATORIEDADE: A criação do ServiceProvider é necessária para gerenciar o ciclo de vida dos objetos no .NET Profissional.
var serviceProvider = new ServiceCollection()
    .AddEstoqueServices()
    .BuildServiceProvider();

// OBRIGATORIEDADE: Você deve resolver o serviço do container para garantir que a injeção ocorra.
var estoqueMicro = serviceProvider.GetRequiredService<Micro.Estoque>();

// OBRIGATORIEDADE: Em um console app, os serviços que apenas observam precisam ser "resolvidos"
// ao menos uma vez para disparar o construtor e, consequentemente, a assinatura (+=).
serviceProvider.GetRequiredService<Micro.MarketingService>();
serviceProvider.GetRequiredService<Micro.ComprasService>();

Console.WriteLine("\n--- [GOOD - MSFT] DI + Events + Extensions ---");
estoqueMicro.AtualizarEstoque("Processador i9", 3);

// ===================================================================================================
// CONCLUSÃO: 
// 1. OBRIGATÓRIO: Uso de 'event', Injeção via Construtor e Registro Singleton do Subject.
// 2. OPCIONAL: Quantidade de parâmetros no Action, nomes dos métodos de notificação e regras de negócio.
// A abordagem Microsoft remove o boilerplate do GoF e foca na fiação (wiring) via infraestrutura de DI.
// ===================================================================================================