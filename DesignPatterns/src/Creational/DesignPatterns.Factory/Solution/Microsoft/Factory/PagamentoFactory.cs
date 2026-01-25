using DesignPatterns.Factory.Solution.Microsoft.Products;
using Microsoft.Extensions.DependencyInjection;

namespace DesignPatterns.Factory.Solution.Microsoft.Factory
{
    public class PagamentoFactory : IPagamentoFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PagamentoFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPagamento Criar(TipoPagamento tipo)
        {
            return _serviceProvider.GetRequiredKeyedService<IPagamento>(tipo);
        }
    }
}
