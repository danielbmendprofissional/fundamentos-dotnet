using DesignPatterns.Factory.Solution.pure.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.pure.Factories
{
    public class BoletoFactory : PagamentoCreator
    {
        public override IPagamento CriarMetodo() => new PagamentoBoleto();
    }
}
