using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.pure.Products
{
    // Products Interface
    public interface IPagamento
    {
        void Processar(decimal valor);
    }
}
