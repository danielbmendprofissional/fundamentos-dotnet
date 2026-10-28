using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Solution.Facade
{
    public interface IVendaFacade
    {
        void ExecutarCompraFluxoSimples(string produto, decimal valor);
    }
}
