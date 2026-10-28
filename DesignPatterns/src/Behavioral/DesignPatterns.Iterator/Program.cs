using Bad = DesignPatterns.Iterator.Bad;
using Gof = DesignPatterns.Iterator.Good.GOF;
using Micro = DesignPatterns.Iterator.Good.Microsoft;

// =============================================== BAD ===============================================

// CASO 1: Acoplamento com Array (Estoque Simples)
var simplesBad = new Bad.EstoqueSimplesBad();
Console.WriteLine("--- [BAD] Percorrendo Array Exposto (Uso de índices) ---");

for (int i = 0; i < simplesBad.Itens.Length; i++)
{
    Console.WriteLine($"- {simplesBad.Itens[i].Nome}");
}

// CASO 2: Acoplamento com Estrutura de Árvore (Complexidade Algorítmica)
var catalogoBad = new Bad.CatalogoArvoreBad();
Console.WriteLine("\n--- [BAD] Percorrendo Árvore (Recursão externa obrigatória) ---");

// OBRIGATORIEDADE: O cliente "sequestra" a lógica de travessia para si.
void MostrarProdutosRecursivo(Bad.Categoria cat)
{
    foreach (var p in cat.Produtos) Console.WriteLine($"- {p.Nome} (na categoria {cat.Nome})");
    foreach (var sub in cat.SubCategorias) MostrarProdutosRecursivo(sub);
}

MostrarProdutosRecursivo(catalogoBad.Raiz);

// =============================================== GOOD ===============================================

// ============================= GOF

// 1. SETUP DE DADOS
var raiz = new Bad.Categoria("Loja");
var hardware = new Bad.Categoria("Hardware");
hardware.Produtos.Add(new Bad.Produto("CPU", 2000));
raiz.SubCategorias.Add(hardware);

// 2. USO DO ITERATOR PARA O ARRAY
var estoqueGof = new Gof.EstoqueSimplesGof();
Gof.Abstractions.IIterator iterSimples = estoqueGof.CreateIterator();

Console.WriteLine("--- [GOOD - GOF] Array via IIterator (Object) ---");
while (iterSimples.HasNext())
{
    var p = (Bad.Produto)iterSimples.Next(); // OBRIGATORIEDADE: Cast manual fiel ao GOF.
    Console.WriteLine($"- {p.Nome}");
}

// 3. USO DO ITERATOR PARA A ÁRVORE
var catalogoGof = new Gof.CatalogoArvoreGof(raiz);
Gof.Abstractions.IIterator iterArvore = catalogoGof.CreateIterator();

Console.WriteLine("\n--- [GOOD - GOF] Árvore via IIterator (Object) ---");
while (iterArvore.HasNext())
{
    var p = (Bad.Produto)iterArvore.Next();
    Console.WriteLine($"- {p.Nome} (sem recursão no cliente)");
}

// 1. SETUP DE DADOS
var raizMicro = new Bad.Categoria("Mega Store");
var hardwareMicro = new Bad.Categoria("Componentes");
hardwareMicro.Produtos.Add(new Bad.Produto("RTX 4090", 11000));
raizMicro.SubCategorias.Add(hardwareMicro);

// 2. USO PROFISSIONAL - ARRAY
var estoqueSimples = new Micro.EstoqueSimplesMicrosoft();

Console.WriteLine("--- [GOOD - MSFT] Array via IEnumerable<T> ---");
foreach (var p in estoqueSimples) // OBRIGATORIEDADE: Consumo tipado e limpo.
{
    Console.WriteLine($"- Produto: {p.Nome}");
}

// 3. USO PROFISSIONAL - ÁRVORE
var catalogoArvore = new Micro.CatalogoArvoreMicrosoft(raizMicro);

Console.WriteLine("\n--- [GOOD - MSFT] Árvore via IEnumerable<T> (Lazy Recursion) ---");
foreach (var p in catalogoArvore)
{
    Console.WriteLine($"- Produto: {p.Nome} (Processado sob demanda)");
}

// ===================================================================================================