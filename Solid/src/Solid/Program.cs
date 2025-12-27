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

//BAD -> Uma classe com varias responsabilidades e várias razões para mudar, problemas futuro de manutenção e escalabilidade de código.

var badReport = new Solid.SRP.Bad.Report();
badReport.Generate();
badReport.Print();
badReport.Save();

//GOOD -> Cada classe com sua responsabilidade definida, melhor para testes, manutenção, interpretação e crescimento da aplicação.
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

//BAD -> varias funcionalidades no mesmo código existente aberto para modificação (se aparecer um novo produto vira um novo if).

var badDesconto = new Solid.OCP.Bad.Discount();
var badValor = badDesconto.Calculate(Solid.OCP.Bad.EProductType.Eletronics, 100);
Console.WriteLine(badValor);

/* 
    GOOD -> Usando do polimorfismo com abstração obrigatória para override obrigando cada produto implementar o seu calculo de desconto,
    agora para criar está aberto a extensão sem modificar os códigos existentes.
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
    BAD -> duas ou uma, ou a base está errada ou o objeto que a herda não deveria herda-la.
    - Caso 1: o BankAccount tem um método nele (WithDraw), mas o another class não o utiliza, "anula" o método da classe pai. Incorreto, irá retornar InvalidOperationException!
    - Caso 2: o BankAccount tem um método nele (WithDraw), mas o Savings class altera o método (override), "invalidando" o já existente do pai. 
*/
//1
Solid.LSP.Bad.BankAccount badAccount1 = new Solid.LSP.Bad.AnotherClass();
badAccount1.Withdraw(100);

//2
Solid.LSP.Bad.BankAccount badAccount2 = new Solid.LSP.Bad.Savings();
badAccount2.Withdraw(100);

/*
    GOOD -> Nesse caso agora o pai obriga o filho a definir o comportamento (evita o caso 2) e para evitar o caso 1 é analisar se o filho realmente deveria estar ali ou se o método do pai
    deveria ou não estar ali.
*/

BankAccount goodAccount1 = new CheckingAccount();
goodAccount1.WithDraw(100);

/* 
 * ===========================================================
 * =========================== ISP ===========================
 * ===========================================================
*/

// BAD -> Cachorro está herdando uma interface com um contrato muito extenso com funções de todos animais, muitas que cachorro nem usa.

var badCachorro = new Solid.ISP.Bad.Cachorro();
badCachorro.Voar(); //cachorro nem voa kkkkk
badCachorro.Nadar(); //cachorro até nada, mas vamos fingir que não

// GOOD -> Agora sim interfaces separadas corretamente por sua função permitindo as classes herdarem seus contratos.

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

// BAD -> Nesse caso o usuário sabe da classe concreta e vai quebrar no e-mail de teste.

var emailService = new Solid.DIP.Bad.EmailService();
var usuario = new Solid.DIP.Bad.UserService(emailService);

var emailServiceTest = new Solid.DIP.Bad.EmailServiceTest();
var usuarioTest = new Solid.DIP.Bad.UserService(emailServiceTest);


// GOOD -> O Usuario só sabe que vai receber um e-mail service pelo contrato, não precisa saber nada da classe concreta.

var goodEmailService = new EmailService();
var goodUsuario = new UserService(goodEmailService);

var goodEmailServiceTest = new EmailServiceTest();
var goodUsuarioTest = new UserService(goodEmailServiceTest);