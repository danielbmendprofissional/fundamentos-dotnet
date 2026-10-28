using DesignPatterns.Facade.Pain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Solution.Facade
{
    public class VendaFacade : IVendaFacade
    {
        private readonly IEstoque _estoque;
        private readonly IPagamento _pagamento;
        private readonly IFiscal _fiscal;
        private readonly ILogistica _logistica;

        // Ele encapsula a complexidade que estava espalhada
        public VendaFacade(IEstoque estoque, IPagamento pagamento, IFiscal fiscal, ILogistica logistica)
        {
            _estoque = estoque;
            _pagamento = pagamento;
            _fiscal = fiscal;
            _logistica = logistica;
        }

        public void ExecutarCompraFluxoSimples(string produto, decimal valor)
        {
            Console.WriteLine(">>> [FACADE] Iniciando orquestração do sistema...");

            _estoque.BaixarItem(produto);
            _pagamento.Processar(valor);
            _fiscal.GerarNota(produto);
            _logistica.AgendarEntrega(produto);

            Console.WriteLine(">>> [FACADE] Fluxo finalizado com sucesso.");
        }
    }
}
