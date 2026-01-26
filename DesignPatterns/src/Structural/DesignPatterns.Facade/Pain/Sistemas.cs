using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Facade.Pain
{
    // --- SUBSISTEMAS ---
    public class EstoqueService : IEstoque
    {
        public void BaixarItem(string produto) => Console.WriteLine($"[Estoque] Item {produto} reservado.");
    }

    public class PagamentoService : IPagamento
    {
        public void Processar(decimal valor) => Console.WriteLine($"[Financeiro] Cobrança de R$ {valor} aprovada.");
    }

    public class FiscalService : IFiscal
    {
        public void GerarNota(string produto) => Console.WriteLine($"[Fiscal] Nota emitida para {produto}.");
    }

    public class LogisticaService : ILogistica
    {
        public void AgendarEntrega(string produto) => Console.WriteLine($"[Logística] Saída para entrega agendada.");
    }
}
