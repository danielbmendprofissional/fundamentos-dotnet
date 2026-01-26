using DesignPatterns.Facade.Solution.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Solution.Service
{
    public class OrdemCompraService : IOrdemCompraService
    {
        private readonly IVendaFacade _vendaFacade;

        public OrdemCompraService(IVendaFacade vendaFacade)
        {
            _vendaFacade = vendaFacade;
        }

        public void ExecutarCompraFluxoSimples(string produto, decimal valor)
        {
            _vendaFacade.ExecutarCompraFluxoSimples(produto, valor);
        }
    }
}
