using DesignPatterns.Facade.Pain;
using DesignPatterns.Facade.Solution.Facade;
using DesignPatterns.Facade.Solution.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrdemCompraService = DesignPatterns.Facade.Solution.Service.OrdemCompraService;

namespace DesignPatterns.Facade.Solution
{
    public static class VendaExtensions
    {
        public static IServiceCollection AddVendaModule(this IServiceCollection services)
        {
            // Escondemos os detalhes técnicos aqui
            services.AddTransient<IEstoque, EstoqueService>();
            services.AddTransient<IPagamento, PagamentoService>();
            services.AddTransient<IFiscal, FiscalService>();
            services.AddTransient<ILogistica, LogisticaService>();

            // Registramos a "Porta de Entrada"
            services.AddScoped<IOrdemCompraService, OrdemCompraService>();
            services.AddScoped<IVendaFacade, VendaFacade>();

            return services;
        }
    }
}
