using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Strategy.Good.GOF.Abstraction
{
    // [Strategy] no seu diagrama.
    // OBRIGATORIEDADE: Seguindo o padrão GOF, deve ser sempre uma Interface ou Classe Abstrata.
    // Esta nomenclatura 'IFreteStrategy' (ou 'IStrategy') é obrigatória para identificar o papel do objeto.
    public interface IFreteStrategy
    {
        // OBRIGATORIEDADE: Este método representa o 'AlgorithmInterface()' do seu diagrama.
        // Ele define como o cálculo será solicitado, independente de qual frete seja.
        double Calcular(double valorPedido);
    }

    // *obs* Em caso de classe abstrata, você teria a mesma assinatura de método, mas abstract
    // tornando obrigatória a implementação do método por herança.
}
