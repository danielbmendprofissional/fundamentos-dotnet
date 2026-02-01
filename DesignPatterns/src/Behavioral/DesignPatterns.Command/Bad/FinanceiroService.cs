using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Command.Bad
{
    public class FinanceiroService
    {
        public void ProcessarPagamento(double valor)
            => Console.WriteLine($"[FINANCEIRO] Pagamento de {valor:C} aprovado.");

        public void CancelarPagamento(double valor)
            => Console.WriteLine($"[FINANCEIRO-ROLLBACK] Estorno de {valor:C} realizado.");
    }
}
