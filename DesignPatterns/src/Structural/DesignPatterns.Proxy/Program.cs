using DesignPatterns.Proxy.Cache;
using DesignPatterns.Proxy.Cache.Bad;
using DesignPatterns.Proxy.Protection;
using DesignPatterns.Proxy.Protection.Bad;
using DesignPatterns.Proxy.Protection.Good;
using DesignPatterns.Proxy.Virtual;
using DesignPatterns.Proxy.Virtual.Good;
using Microsoft.Extensions.Caching.Memory;
using DesignPatterns.Proxy.Cache.Good;
using Microsoft.Extensions.DependencyInjection;

// ====================================== VIRTUAL ======================================
Console.WriteLine("Iniciando Dashboard...");

// Objeto criado, mas a memória está limpa e o sistema está rápido.
IRelatorio relatorio = new RelatorioProxy();

Console.WriteLine("Deseja ver o relatório agora? (S/N)");
if (Console.ReadKey().Key == ConsoleKey.S)
{
    relatorio.Exibir(); // A lentidão só acontece aqui.
}

// ====================================== PROTEÇÃO ======================================

// At Production code, we always use Interfaces to refer to the services, so the Client is juts
// consuming IProntuarioService, he never knows if it's a Proxy or the Real Service.

// --- SETUP ---
var repo = new ProntuarioRepository();
// A Service Real precisa do repo
var serviceReal = new ProntuarioServiceReal(repo);

// --- CENÁRIO 1: Dr. João (ID 1) tenta deletar o SEU prontuário (55) ---
Console.WriteLine("CENÁRIO 1: Dr. João acessando seu próprio dado.");
var proxyJoao = new ProntuarioProxy(serviceReal, repo, 1);
proxyJoao.DeletarProntuario(55);

Console.WriteLine("\n--------------------------------------------\n");

// --- CENÁRIO 2: Dr. João (ID 1) tenta deletar o prontuário do Dr. Rodrigo (99) ---
Console.WriteLine("CENÁRIO 2: Dr. João tentando acessar dado de outro médico.");
var proxyInvasor = new ProntuarioProxy(serviceReal, repo, 1);
proxyInvasor.DeletarProntuario(99);

// ======================================== CACHE ========================================

// 1. Configurando o Container de DI (O "Cérebro" da aplicação)
var services = new ServiceCollection();
services.AddMemoryCache(); // Importante: Ativa o IMemoryCache da Microsoft

// 2. Registro do Proxy (Como fizemos na ServiceExtensions)
services.AddScoped<CotacaoServiceReal>();
services.AddScoped<ICotacaoService>(provider => {
    var real = provider.GetRequiredService<CotacaoServiceReal>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CotacaoCacheProxy(real, cache);
});

var serviceProvider = services.BuildServiceProvider();
var proxy = serviceProvider.GetRequiredService<ICotacaoService>();

// --- TESTE DE FOGO ---

Console.WriteLine("--- PRIMEIRA CHAMADA (Vai demorar) ---");
Console.WriteLine($"Resultado: {proxy.ObterPrecoDolar()}");

Console.WriteLine("\n--- SEGUNDA CHAMADA (Instantânea - Cache ativo) ---");
Console.WriteLine($"Resultado: {proxy.ObterPrecoDolar()}");

Console.WriteLine("\n--- AGUARDANDO 11 SEGUNDOS PARA EXPIRAR... ---");
Thread.Sleep(11000);

Console.WriteLine("\n--- TERCEIRA CHAMADA (Vai demorar de novo - Cache expirou) ---");
Console.WriteLine($"Resultado: {proxy.ObterPrecoDolar()}");