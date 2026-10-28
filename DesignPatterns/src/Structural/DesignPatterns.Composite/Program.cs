using DesignPatterns.Composite;
using DesignPatterns.Composite.FirstExample;
using DesignPatterns.Composite.SecondExample;
using DesignPatterns.Composite.ThirdExample;
using DesignPatterns.Composite.ThirdExample.Factory;
using Microsoft.Extensions.DependencyInjection;

// ==================================================== FIRST EXAMPLE ====================================================

// Criando as folhas (Leafs)
var mouse = new ProductLeaf("Gaming Mouse", 300.00m);
var keyboard = new ProductLeaf("Mechanical Keyboard", 500.00m);
var monitor = new ProductLeaf("4K Monitor", 2500.00m);

// Criando o Composite (Kit de Periféricos)
var peripheralsKit = new ProductKitComposite("PC Peripherals Kit");
peripheralsKit.Add(mouse);
peripheralsKit.Add(keyboard);

// Criando o Composite principal (Setup Completo)
var fullSetup = new ProductKitComposite("Full Gamer Setup");
fullSetup.Add(monitor);
fullSetup.Add(peripheralsKit); // Kit dentro de Kit

// EXECUTION - O cliente trata tudo como ISellable
PrintReceipt(fullSetup);
PrintReceipt(new ProductLeaf("Individual Mouse Pad", 50.00m));

void PrintReceipt(ISellable item)
{
    Console.WriteLine($"Product: {item.GetName()} | Total: {item.GetPrice():C}");
}

// =================================================== SECOND EXAMPLE ====================================================

// --- PAIN: Building a complex hierarchy manually ---

var iphone = new IndividualPackageLeaf("iPhone 15", 0.2);
var macbook = new IndividualPackageLeaf("MacBook Pro", 1.5);

var electronicsBox = new ShippingContainerComposite("EB-01");
electronicsBox.Add(iphone);
electronicsBox.Add(macbook);

var sofa = new IndividualPackageLeaf("Leather Sofa", 50.0);
var mainCrate = new ShippingContainerComposite("Crate-99");
mainCrate.Add(sofa);
mainCrate.Add(electronicsBox);

// --- EXECUTION ---
PrintCargoDetails(mainCrate);

void PrintCargoDetails(ILogisticsItem item)
{
    Console.WriteLine($"Item: {item.GetDescription()}");
    Console.WriteLine($"Total Weight: {item.GetWeight()} kg");
}

// ==================================================== THIRD EXAMPLE ====================================================
// REAL SCENARIO: Authorization Policies with Factory and DI

var services = new ServiceCollection();
services.AddCustomAuthorization(); // Registering the Factory
var provider = services.BuildServiceProvider();

// 1. Resolve the Factory
var authFactory = provider.GetRequiredService<IPermissionFactory>();

// 2. Create complex policies based on runtime parameters (No "Default" method)
var managerPolicy = authFactory.CreatePolicy("Manager");
var adminPolicy = authFactory.CreatePolicy("SuperAdmin");

// 3. Execution - Client doesn't know if it's a single permission or a tree
CheckAccess(managerPolicy, "Manager");
CheckAccess(adminPolicy, "Editor");

void CheckAccess(IAuthorizationRequirement requirement, string userRole)
{
    Console.WriteLine($"\nPolicy: {requirement.GetName()}");
    Console.WriteLine($"Checking access for role: {userRole}");

    if (requirement.IsAuthorized(userRole))
        Console.WriteLine(">>> Result: ACCESS GRANTED");
    else
        Console.WriteLine(">>> Result: ACCESS DENIED");
}