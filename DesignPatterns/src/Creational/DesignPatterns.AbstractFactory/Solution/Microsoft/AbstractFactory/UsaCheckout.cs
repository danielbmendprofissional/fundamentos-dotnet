using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory
{
    internal class UsaCheckout : IInternacionalCheckout
    {
        private readonly IServiceProvider _serviceProvider;

        // A Microsoft injeta o motor de busca (IServiceProvider) aqui
        public UsaCheckout(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IPagamento CriarPagamento(TipoPagamento tipo) =>
            _serviceProvider.GetRequiredKeyedService<IPagamento>( (Pais.Usa, tipo) );

        public IDocumento CriarDocumento() =>
            _serviceProvider.GetRequiredKeyedService<IDocumento>(TipoDocumento.UsaInvoice);
    }
}
