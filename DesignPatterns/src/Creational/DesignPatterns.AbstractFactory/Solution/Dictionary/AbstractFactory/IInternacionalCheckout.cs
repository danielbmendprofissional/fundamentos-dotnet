using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory
{
    public interface IInternacionalCheckout
    {
        IPagamento CriarPagamento(string tipo);
        IDocumento CriarDocumento();
    }
}
