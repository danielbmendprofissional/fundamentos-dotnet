using Bad = DesignPatterns.Flyweight.Bad;
using Gof = DesignPatterns.Flyweight.Good.GOF;
using Micro = DesignPatterns.Flyweight.Good.Microsoft;

// =============================================== BAD ===============================================
// Cenário onde cada instância carrega seus próprios bytes.
// Se criarmos 1000 itens de 2MB cada, tentaremos alocar 2GB de RAM.

var inventoryBad = new List<Bad.Item>();

Console.WriteLine("--- [BAD] Iniciando Alocação (Cenário Ruim) ---");
long memBeforeBad = GC.GetTotalMemory(true);

for (int i = 0; i < 1000; i++)
{
    // CADA item aqui dentro cria sua própria cópia física dos bytes.
    var item = new Bad.Item(
        "Espada de Platina",
        new byte[1_000_000], // 1MB
        new byte[1_000_000], // 1MB
        Guid.NewGuid(),
        "Player_Bad"
    );
    inventoryBad.Add(item);
}

long memAfterBad = GC.GetTotalMemory(true);
double totalMBBad = (memAfterBad - memBeforeBad) / 1024.0 / 1024.0;
Console.WriteLine($"[BAD] Itens: {inventoryBad.Count} | Memória: {totalMBBad:F2} MB\n");


// =============================================== GOOD ===============================================
// Preparação de dados comuns para os testes Good
byte[] sharedIcon = new byte[1_000_000];
byte[] sharedModel = new byte[1_000_000];

// ================ 1. GOF (Fidelidade Acadêmica)
// Foco: COMPORTAMENTO. O objeto 'Item' não te dá os bytes, ele executa a ação.

Console.WriteLine("--- [GOOD - GOF] Iniciando (Fidelidade Acadêmica) ---");
var gofFactory = new Gof.ItemFactory();
var inventoryGof = new List<Gof.Item>();
long memBeforeGof = GC.GetTotalMemory(true);

// Obtemos o "Flyweight" (Peso Pesado) uma única vez
var gofType = gofFactory.GetFlyweight("Platina", sharedIcon, sharedModel);

for (int i = 0; i < 1000; i++)
{
    // Criamos 1000 contextos leves que APONTAM para o mesmo Flyweight
    var item = new Gof.Item(Guid.NewGuid(), "Player_Gof", "Espada", "Desc", gofType);
    inventoryGof.Add(item);
}

long memAfterGof = GC.GetTotalMemory(true);
double totalMBGof = (memAfterGof - memBeforeGof) / 1024.0 / 1024.0;

// No GOF, você não acessa item.IconData. Você manda ele se comportar:
inventoryGof[0].Display();

Console.WriteLine($"[GOF] Itens: {inventoryGof.Count} | Memória: {totalMBGof:F2} MB\n");


// ================ 2. MICROSOFT (Fidelidade .NET)
// Foco: TRANSPARÊNCIA DE DADOS. O objeto 'Item' expõe os bytes via propriedades (Proxy).

Console.WriteLine("--- [GOOD - MSFT] Iniciando (Fidelidade .NET) ---");
var msftFactory = new Micro.ItemFactory();
var inventoryMsft = new List<Micro.Item>();
long memBeforeMsft = GC.GetTotalMemory(true);

// Registramos o DNA pesado
var msftType = msftFactory.GetFlyweight("Platina", sharedIcon, sharedModel);

for (int i = 0; i < 1000; i++)
{
    // Criamos 1000 objetos que parecem completos, mas são leves
    var item = new Micro.Item(Guid.NewGuid(), "Player_Msft", "Espada", "Desc", msftType);
    inventoryMsft.Add(item);
}

long memAfterMsft = GC.GetTotalMemory(true);
double totalMBMsft = (memAfterMsft - memBeforeMsft) / 1024.0 / 1024.0;

// No .NET, você ACESSA a propriedade diretamente, de forma transparente:
var iconSize = inventoryMsft[0].IconData.Length;

Console.WriteLine($"[MSFT] Itens: {inventoryMsft.Count} | Memória: {totalMBMsft:F2} MB");
Console.WriteLine($"[MSFT] Verificação de Acesso Direto: IconSize = {iconSize} bytes");

// ===================================================================================================
// CONCLUSÃO: 
// O cenário BAD gastou ~2000MB (2GB).
// O cenário GOF e MSFT gastaram apenas os 2MB iniciais + o overhead mínimo das classes leves.
// ===================================================================================================