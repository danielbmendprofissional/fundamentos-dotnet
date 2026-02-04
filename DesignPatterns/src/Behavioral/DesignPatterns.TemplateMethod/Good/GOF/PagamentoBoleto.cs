using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.TemplateMethod.Good.GOF
{
    // OBRIGATORIEDADE: Classe Concreta.
    public class PagamentoBoleto : ProcessadorPagamento
    {
        protected override void OperationProcessar()
            => Console.WriteLine("[Boleto] Gerando código de barras para o banco.");
    }
}
