using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.TemplateMethod.Good.GOF
{
    public class PagamentoCartao : ProcessadorPagamento
    {
        protected override void OperationProcessar()
            => Console.WriteLine("[Cartão] Solicitando autorização da operadora.");
    }
}
