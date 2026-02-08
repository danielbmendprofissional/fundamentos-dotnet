using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Bad = DesignPatterns.ChainOfResponsibility.Bad;
using Gof = DesignPatterns.ChainOfResponsibility.Good.GOF;
using Micro = DesignPatterns.ChainOfResponsibility.Good.Microsoft;

// =============================================== BAD ===============================================

Console.WriteLine("--- [BAD] Processador com IFs (Acoplado) ---");
var processadorBad = new Bad.ProcessadorDescontoBad();
processadorBad.Aprovar(500); // Lógica de decisão escondida dentro de um único método gigante.

// =============================================== GOOD ===============================================

// ============================= GOF
Console.WriteLine("\n--- [GOOD - GOF] Chain of Responsibility (Corrente) ---");

// 1. Instanciamos os elos
var vendedor = new Gof.Vendedor();
var gerente = new Gof.Gerente();
var diretor = new Gof.Diretor();

// 2. Montamos a corrente (Vendedor -> Gerente -> Diretor)
vendedor.SetSucessor(gerente);
gerente.SetSucessor(diretor);

// 3. O cliente fala apenas com o primeiro elo.
// A requisição 'anda' sozinha até encontrar quem a resolva.
Console.WriteLine("Solicitando desconto de R$ 50:");
vendedor.ProcessarSolicitacao(50);

Console.WriteLine("\nSolicitando desconto de R$ 500:");
vendedor.ProcessarSolicitacao(500);

Console.WriteLine("\nSolicitando desconto de R$ 3000:");
vendedor.ProcessarSolicitacao(3000);

// ============================= MICROSOFT

// ========== MIDDLEWARE

Console.WriteLine("\n--- [GOOD - MICROSOFT] Pipeline com Terminal (Diretor) ---");

var services = new ServiceCollection();
services.AddTransient<Micro.Middleware.VendedorMiddleware>();
services.AddTransient<Micro.Middleware.GerenteMiddleware>();
services.AddTransient<Micro.Middleware.DiretorMiddleware>(); // O Diretor Terminal

var provider = services.BuildServiceProvider();

// Simulação de montagem de Pipeline (O que o ASP.NET faz por debaixo do capô)
var context1 = new Micro.Middleware.DescontoContext(5000); // Valor alto para chegar no Diretor

var vendedor1 = provider.GetRequiredService<Micro.Middleware.DiretorMiddleware>();
var gerente1 = provider.GetRequiredService<Micro.Middleware.DiretorMiddleware>();
var diretor1 = provider.GetRequiredService<Micro.Middleware.DiretorMiddleware>();

await vendedor1.InvokeAsync(context1, async (ctx) =>
{
    await gerente1.InvokeAsync(ctx, async (c) =>
    {
        // O gerente chama o diretor como sendo o seu 'next'
        await diretor1.InvokeAsync(c, null);
    });
});

// ========== PIPELINE BEHAVIOR

var services2 = new ServiceCollection();

// OBRIGATORIEDADE: A ordem de registro define a ordem da corrente!
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    // Montando a corrente: Vendedor -> Gerente -> (DiretorHandler está implícito)
    cfg.AddOpenBehavior(typeof(Micro.MediatR.VendedorBehavior<,>));
    cfg.AddOpenBehavior(typeof(Micro.MediatR.GerenteBehavior<,>));
});

var provider2 = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();

// Teste 1: Vendedor barra
var result1 = await mediator.Send(new Micro.MediatR.Command.AprovarDescontoCommand(50));
Console.WriteLine($"Resultado: {result1}");

// Teste 2: Gerente barra
var result2 = await mediator.Send(new Micro.MediatR.Command.AprovarDescontoCommand(500));
Console.WriteLine($"Resultado: {result2}");

// Teste 3: Chega no Diretor (Fim da linha)
var result3 = await mediator.Send(new Micro.MediatR.Command.AprovarDescontoCommand(5000));
Console.WriteLine($"Resultado: {result3}");

// ===================================================================================================
// ===================================================================================================