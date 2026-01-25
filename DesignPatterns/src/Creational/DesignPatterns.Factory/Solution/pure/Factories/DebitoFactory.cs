using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Factory.Solution.pure.Factories
{
    public class DebitoFactory : PagamentoCreator
    {
        public override Products.IPagamento CriarMetodo() => new Products.PagamentoDebito();
    }
}
