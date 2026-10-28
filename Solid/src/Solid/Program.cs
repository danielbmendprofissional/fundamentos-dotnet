using Solid.DIP.Good;
using Solid.ISP.Good;
using Solid.LSP.Good;
using Solid.OCP.Good;
using Solid.SRP.Good;

/* 
 * ===========================================================
 * =========================== SRP ===========================
 * ===========================================================
*/

// BAD -> A class with multiple responsibilities and multiple reasons to change, leading to future maintenance and scalability issues.

var badReport = new Solid.SRP.Bad.Report();
badReport.Generate();
badReport.Print();
badReport.Save();

// GOOD -> Each class has its defined responsibility, making testing, maintenance, interpretation, and application growth easier.
var goodReport = new Report();
goodReport.Generate();

var printService = new PrintService();
printService.Print(goodReport);

var reportRepository = new ReportRepository();
reportRepository.Save(goodReport);


/* 
 * ===========================================================
 * =========================== OCP ===========================
 * ===========================================================
*/

// BAD -> Multiple functionalities inside the same existing code, open to modification (if a new product appears, another if is added).

var badDesconto = new Solid.OCP.Bad.Discount();
var badValor = badDesconto.Calculate(Solid.OCP.Bad.EProductType.Eletronics, 100);
Console.WriteLine(badValor);

/* 
    GOOD -> Using polymorphism with mandatory abstraction and override, forcing each product to implement its own discount calculation.
    Now the system is open to extension without modifying existing code.
*/

var beautyDesconto = new BeautyDiscount();
var eletronicDesconto = new BeautyDiscount();

var beautyValor = beautyDesconto.Calculate(100);
var eletronicValor = eletronicDesconto.Calculate(200);

Console.WriteLine(beautyValor);
Console.WriteLine(eletronicValor);

/* 
 * ===========================================================
 * =========================== LSP ===========================
 * ===========================================================
*/

/*
    BAD -> Either the base class is wrong or the object inheriting it should not inherit at all.
    - Case 1: BankAccount has a method (Withdraw), but another class doesn’t use it, “nullifying” the parent method. Incorrect, will throw InvalidOperationException!
    - Case 2: BankAccount has a method (Withdraw), but Savings overrides it, invalidating the parent’s existing behavior.
*/
// 1
Solid.LSP.Bad.BankAccount badAccount1 = new Solid.LSP.Bad.AnotherClass();
badAccount1.Withdraw(100);

// 2
Solid.LSP.Bad.BankAccount badAccount2 = new Solid.LSP.Bad.Savings();
badAccount2.Withdraw(100);

/*
    GOOD -> In this case, the parent forces the child to define the behavior (avoids case 2).
    To avoid case 1, analyze whether the child should really inherit or whether the parent’s method belongs there.
*/

BankAccount goodAccount1 = new CheckingAccount();
goodAccount1.WithDraw(100);

/* 
 * ===========================================================
 * =========================== ISP ===========================
 * ===========================================================
*/

// BAD -> Dog inherits an interface with an overly extensive contract containing functions for all animals, many of which dogs don’t use.

var badCachorro = new Solid.ISP.Bad.Cachorro();
badCachorro.Voar(); // dogs don’t fly lol
badCachorro.Nadar(); // dogs can swim, but let’s pretend they don’t

// GOOD -> Interfaces are now properly separated by function, allowing classes to inherit only the contracts they need.

var goodCachorro = new Cachorro();
goodCachorro.Andar();
goodCachorro.Latir();

var goodPombo = new Pombo();
goodPombo.Voar();
goodPombo.Ciscar();

/* 
 * ===========================================================
 * =========================== DIP ===========================
 * ===========================================================
*/

// BAD -> In this case, the user knows the concrete class, which will break in the test email scenario.

var emailService = new Solid.DIP.Bad.EmailService();
var usuario = new Solid.DIP.Bad.UserService(emailService);

var emailServiceTest = new Solid.DIP.Bad.EmailServiceTest();
var usuarioTest = new Solid.DIP.Bad.UserService(emailServiceTest);


// GOOD -> The User only knows it will receive an email service via the contract, without needing to know the concrete class.

var goodEmailService = new EmailService();
var goodUsuario = new UserService(goodEmailService);

var goodEmailServiceTest = new EmailServiceTest();
var goodUsuarioTest = new UserService(goodEmailServiceTest);