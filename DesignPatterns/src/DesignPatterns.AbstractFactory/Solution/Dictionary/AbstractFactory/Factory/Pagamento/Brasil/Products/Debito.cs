using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento.Brasil.Products
{
    // Concrete Product
    public class Debito : IPagamento
    {
        public void Processar(decimal valor)
        {
            Console.WriteLine("Processando pagamento com cartão de débito...");
        }
    }
}
