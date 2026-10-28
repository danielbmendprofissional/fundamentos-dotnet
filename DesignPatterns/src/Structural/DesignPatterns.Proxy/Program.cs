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
using DesignPatterns.Proxy.Remote;

// ====================================== VIRTUAL ======================================
Console.WriteLine("Starting Dashboard...");

// Object created, but memory is clean and the system is fast.
IRelatorio relatorio = new RelatorioProxy();

Console.WriteLine("Do you want to see the report now? (Y/N)");
if (Console.ReadKey().Key == ConsoleKey.Y)
{
    relatorio.Exibir(); // The slowness only happens here.
}

// ====================================== PROTECTION ======================================

// At Production code, we always use Interfaces to refer to the services, so the Client is just
// consuming IProntuarioService, he never knows if it's a Proxy or the Real Service.

// --- SETUP ---
var repo = new ProntuarioRepository();
// Real Service needs the repo
var serviceReal = new ProntuarioServiceReal(repo);

// --- SCENARIO 1: Dr. João (ID 1) tries to delete HIS OWN record (55) ---
Console.WriteLine("SCENARIO 1: Dr. João accessing his own data.");
var proxyJoao = new ProntuarioProxy(serviceReal, repo, 1);
proxyJoao.DeletarProntuario(55);

Console.WriteLine("\n--------------------------------------------\n");

// --- SCENARIO 2: Dr. João (ID 1) tries to delete Dr. Rodrigo's record (99) ---
Console.WriteLine("SCENARIO 2: Dr. João trying to access another doctor's data.");
var proxyInvasor = new ProntuarioProxy(serviceReal, repo, 1);
proxyInvasor.DeletarProntuario(99);

// ======================================== CACHE ========================================

// 1. Configuring the DI Container (The application's "Brain")
var services = new ServiceCollection();
services.AddMemoryCache(); // Important: Activates Microsoft's IMemoryCache

// 2. Proxy Registration (As we did in ServiceExtensions)
services.AddScoped<CotacaoServiceReal>();
services.AddScoped<ICotacaoService>(provider => {
    var real = provider.GetRequiredService<CotacaoServiceReal>();
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new CotacaoCacheProxy(real, cache);
});

var serviceProvider = services.BuildServiceProvider();
var proxy = serviceProvider.GetRequiredService<ICotacaoService>();

// --- ACID TEST ---

Console.WriteLine("--- FIRST CALL (Will be slow) ---");
Console.WriteLine($"Result: {proxy.ObterPrecoDolar()}");

Console.WriteLine("\n--- SECOND CALL (Instant - Cache active) ---");
Console.WriteLine($"Result: {proxy.ObterPrecoDolar()}");

Console.WriteLine("\n--- WAITING 11 SECONDS FOR EXPIRATION... ---");
Thread.Sleep(11000);

Console.WriteLine("\n--- THIRD CALL (Will be slow again - Cache expired) ---");
Console.WriteLine($"Result: {proxy.ObterPrecoDolar()}");


// ====================================== REMOTE ======================================
var remoteServices = new ServiceCollection();
remoteServices.AddWeatherAcl();

var remoteProvider = remoteServices.BuildServiceProvider();
var weatherService = remoteProvider.GetRequiredService<IWeatherService>();

var weather = weatherService.GetTemperatureCelsius("London");
Console.WriteLine($"\nTemperature in London: {weather} °C");