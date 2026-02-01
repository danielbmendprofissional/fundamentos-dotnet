using DesignPatterns.Strategy.Good.Microsoft.Abstraction;

namespace DesignPatterns.Strategy.Good.Microsoft
{
    public class FreteInternacionalStrategy : IFreteStrategy
    {
        public double Calcular(double valorPedido)
        {
            // EXEMPLO OPCIONAL: Regra de negócio específica.
            return (valorPedido * 0.5) + 50;
        }
    }
}
