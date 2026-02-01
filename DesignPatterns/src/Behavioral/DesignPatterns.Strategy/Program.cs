using DesignPatterns.Strategy.Good;
using Microsoft.Extensions.DependencyInjection;
using Bad = DesignPatterns.Strategy.Bad;
using Gof = DesignPatterns.Strategy.Good.GOF;
using Micro = DesignPatterns.Strategy.Good.Microsoft;

// =============================================== BAD ===============================================

var pedidoBad = new Bad.Pedido { ValorProduto = 100.0 };
Console.WriteLine($"--- [BAD] Total: {pedidoBad.CalcularTotalComFrete(Bad.TipoFrete.Normal):C}");

// =============================================== GOOD ===============================================

// ============================= GOF

var strategyNormal = new Gof.FreteNormalStrategy();
var pedidoGof = new Gof.Pedido(strategyNormal) { ValorProduto = 100.0 };
Console.WriteLine($"\n--- [GOOD - GOF] Total: {pedidoGof.CalcularTotal():C}");

// ============================= MICROSOFT

var serviceProvider = new ServiceCollection()
    .AddMicrosoftStrategyServices()
    .BuildServiceProvider();

// EXEMPLO OPCIONAL: Imagine que este dado venha de uma escolha do usuário na tela.
Bad.TipoFrete tipoEscolhidoPeloUsuario = Bad.TipoFrete.Expresso;

// OBRIGATORIEDADE: Usamos 'GetRequiredKeyedService' para pedir a implementação 
// que foi amarrada àquela chave específica no registro.
// Isso elimina qualquer 'if' ou 'foreach' no seu código de consumo.
var strategy = serviceProvider.GetRequiredKeyedService<Micro.Abstraction.IFreteStrategy>(tipoEscolhidoPeloUsuario);

double resultado = strategy.Calcular(100.0);

Console.WriteLine($"--- [MSFT .NET 8+] Keyed Service ---");
Console.WriteLine($"Estratégia selecionada via chave: {strategy.GetType().Name}");
Console.WriteLine($"Resultado: {resultado:C}");

// ===================================================================================================
// CONCLUSÃO: 
// A versão Microsoft utiliza o Container de DI para carregar todas as 
// estratégias. A grande vantagem é que o Pedido não precisa nem saber 
// que as estratégias existem; a infraestrutura de serviços resolve 
// dinamicamente qual algoritmo aplicar com base no Tipo.
// ===================================================================================================