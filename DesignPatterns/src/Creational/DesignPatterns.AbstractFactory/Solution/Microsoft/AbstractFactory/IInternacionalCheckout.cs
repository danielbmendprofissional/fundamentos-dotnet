using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory
{
    public interface IInternacionalCheckout
    {
        IPagamento CriarPagamento(TipoPagamento tipo);
        IDocumento CriarDocumento();
    }
}
