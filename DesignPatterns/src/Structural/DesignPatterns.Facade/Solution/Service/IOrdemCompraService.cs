using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Solution.Service
{
    public interface IOrdemCompraService
    {
        void ExecutarCompraFluxoSimples(string produto, decimal valor);
    }
}
