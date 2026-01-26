using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Pain
{
    public class OrdemCompraService
    {
        private readonly IEstoque _estoque;
        private readonly IPagamento _pagamento;
        private readonly IFiscal _fiscal;
        private readonly ILogistica _logistica;

        // CONSTRUTOR INCHADO: Ruim de manter e ruim de testar
        public OrdemCompraService(IEstoque estoque, IPagamento pagamento, IFiscal fiscal, ILogistica logistica)
        {
            _estoque = estoque;
            _pagamento = pagamento;
            _fiscal = fiscal;
            _logistica = logistica;
        }

        public void FinalizarVenda(string produto, decimal valor)
        {
            Console.WriteLine("--- Iniciando Processo de Venda ---");
            _estoque.BaixarItem(produto);
            _pagamento.Processar(valor);
            _fiscal.GerarNota(produto);
            _logistica.AgendarEntrega(produto);
            Console.WriteLine("--- Venda Finalizada com Sucesso ---");
        }
    }
}
