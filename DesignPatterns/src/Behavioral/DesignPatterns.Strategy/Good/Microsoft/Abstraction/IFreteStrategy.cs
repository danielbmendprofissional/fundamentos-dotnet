using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good.Microsoft.Abstraction
{
    // OBRIGATORIEDADE: A interface permanece como o contrato do algoritmo (GOF).
    // Em .NET, o prefixo 'I' é mandatório.
    public interface IFreteStrategy
    {
        // OBRIGATORIEDADE: Representa o 'AlgorithmInterface()' do diagrama.
        double Calcular(double valorPedido);
    }
}
