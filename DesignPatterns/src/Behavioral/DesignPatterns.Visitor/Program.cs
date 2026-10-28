using Bad = DesignPatterns.Visitor.Bad;
using Gof = DesignPatterns.Visitor.Good.GOF;
using Micro = DesignPatterns.Strategy.Good.Microsoft;

// =============================================== BAD ===============================================

Console.WriteLine("--- [BAD] Visitor - Entidades Infladas e Acopladas ---");

// OBRIGATORIEDADE: Como não há interface comum para as operações extras (PDF/Imposto),
// o cliente precisa lidar com tipos concretos ou interfaces genéricas demais.
var carrinho = new List<object>
{
    new Bad.Livro { Titulo = "C# Profissional", Preco = 120 },
    new Bad.Eletronico { Nome = "Mouse Gamer", Preco = 300 }
};

foreach (var item in carrinho)
{
    // OBRIGATORIEDADE: O temido "Type Checking" manual.
    // Se surgir um novo produto (ex: Alimento), você PRECISA mudar esse loop.
    if (item is Bad.Livro l)
    {
        l.GerarRelatorioPDF();
        Console.WriteLine($"Imposto Livro: {l.CalcularImposto()}");
    }
    else if (item is Bad.Eletronico e)
    {
        e.GerarRelatorioPDF();
        Console.WriteLine($"Imposto Eletrônico: {e.CalcularImposto()}");
    }
}

// =============================================== GOOD ===============================================

// ============================= GOF

Console.WriteLine("--- [GOOD - GOF] Visitor (Double Dispatch) ---");

// OBRIGATORIEDADE: ObjectStructure do diagrama.
// Uma coleção que mantém objetos do tipo Element (IProduto).
var carrinho1 = new List<Gof.Interface.IProduto>
{
    new Gof.Livro { Titulo = "Design Patterns", Preco = 100 },
    new Gof.Eletronico { Nome = "Smartphone", Preco = 2000 }
};

// OBRIGATORIEDADE: O Client (este escopo) escolhe qual ConcreteVisitor usar.
Gof.Interface.IVisitor impostoAlgoritmo = new Gof.ImpostoVisitor();
Gof.Interface.IVisitor pdfAlgoritmo = new Gof.PDFVisitor();

// A mágica do diagrama acontece aqui:
// 1. O Client percorre a ObjectStructure.
// 2. Chama Accept() no Element.
// 3. O Element faz o callback para o Visit() correto do Visitor selecionado.
Console.WriteLine("Executando Operação 1 (Impostos):");
carrinho1.ForEach(item => item.Accept(impostoAlgoritmo));

Console.WriteLine("\nExecutando Operação 2 (PDF):");
carrinho1.ForEach(item => item.Accept(pdfAlgoritmo));

// ============================= MICROSOFT

var livro = new Micro.Livro("C# em Ação", 100);
var imposto = livro.GetTax();

// ===================================================================================================