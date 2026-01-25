using DesignPatterns.Factory.Solution.Microsoft.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.Microsoft.Factory
{
    public interface IPagamentoFactory
    {
        IPagamento Criar(TipoPagamento tipo);
    }
}
