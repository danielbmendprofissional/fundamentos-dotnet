using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Bad = DesignPatterns.Mediator.Bad;
using Gof = DesignPatterns.Mediator.Good.GOF;
using Micro = DesignPatterns.Mediator.Good.Microsoft;
using DesignPatterns.Mediator;

// =============================================== BAD ===============================================

var checkoutBad = new Bad.CarrinhoBad();
checkoutBad.FinalizarCompra("Teclado Gamer", 450.00m);

// =============================================== GOOD ===============================================

// ============================= GOF

var mediatorGof = new Gof.CheckoutMediator();
var carrinhoGof = new Gof.Carrinho(mediatorGof);
var estoqueGof = new Gof.Estoque(mediatorGof);
var financeiroGof = new Gof.Financeiro(mediatorGof);

mediatorGof.SetComponents(carrinhoGof, estoqueGof, financeiroGof);

Console.WriteLine("\n--- [GOOD - GOF] Iniciando via Mediador (Notify com IFs) ---");
carrinhoGof.Finalizar();

// ============================= MICROSOFT

// 1. SETUP DA INFRAESTRUTURA (DI)
var services = new ServiceCollection();
services.AddMicrosoft(); // OBRIGATORIEDADE: ServiceExtension profissional.
var serviceProvider = services.BuildServiceProvider();

// 2. OBTENDO O MEDIADOR
var mediatorMs = serviceProvider.GetRequiredService<IMediator>();

Console.WriteLine("\n--- [GOOD - MICROSOFT] Iniciando via MediatR (Handlers Isolados) ---");

// 3. EXECUÇÃO
// OBRIGATORIEDADE: O cliente envia um comando. O MediatR sabe exatamente quem deve tratar.
await mediatorMs.Send(new Micro.CheckoutCommand("Monitor 4K", 3000m));

// ===================================================================================================