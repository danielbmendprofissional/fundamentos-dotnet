// --- PURE ---

using DesignPatterns.Factory.Solution.enumWay;
using DesignPatterns.Factory.Solution.pure;
using DesignPatterns.Factory.Solution.pure.Factories;
using DesignPatterns.Factory.Solution.pure.Products;

Console.WriteLine("=== SISTEMA DE CHECKOUT ===");
Console.WriteLine("Escolha o método: 1-Pix | 2-Cartão");
var escolha = Console.ReadLine();

// In a API we could use Dependency Injection to resolve the factory
// or just use endpoint routing to different factories based on the route called.
// Here we are simulating user choice at runtime...

PagamentoCreator meuGateway = escolha switch
{
    "1" => new PixFactory(),
    "2" => new CreditoFactory(),
    _ => throw new Exception("Opção inválida!")
};

ProcessarPedido(meuGateway, 150.50m);


void ProcessarPedido(PagamentoCreator gateway, decimal total)
{
    Console.WriteLine("Iniciando processamento do pedido...");

    // They don't know which factory they are using, but they know it follows the same interface and works.
    gateway.FinalizarVenda(total);

    Console.WriteLine("Pedido finalizado com sucesso!");
}

// =========================================================================
// --- ENUM ---

// In .NET we see alot of factories implemented using Enums.
// But depends on the scenario is bad practice, because every time you add a new product
// you need to modify the factory code, violating OCP (Open Closed Principle),
// but is less code.

Console.WriteLine("--- CHECKOUT RÁPIDO ---");
Console.WriteLine("Escolha: 1-Credito | 2-Débito | 3-Pix | 4-Boleto");
var entrada = Console.ReadLine();
var entradaInt = int.Parse(entrada);

IPagamento pagamento = PagamentoFactory.Create((MetodoPagamento)entradaInt);

pagamento.Processar(250.00m);
