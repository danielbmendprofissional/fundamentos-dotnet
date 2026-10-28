using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory.Factory.Pagamento
{
    // Product Abstrato
    public interface IPagamento
    {
        void Processar(decimal valor);
    }
}
