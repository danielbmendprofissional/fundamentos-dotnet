using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Application.Pedidos.Interfaces;
using Vendas.Domain.Catalogo.Interfaces.Categorias;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Infrastructure.Catalogo.Repositories;
using Vendas.Infrastructure.Common.Base;
using Vendas.Infrastructure.Common.Persistence;
using Vendas.Infrastructure.Pedidos.Repositories;

namespace Vendas.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructureApp(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<VendasDbContext>(opt => opt.UseSqlite(connectionString));
            services.AddDbContext<VendasQueryDbContext>(opt => opt.UseSqlite(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPedidoQueryRepository, PedidoQueryRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            services.AddScoped<IIntegrationEventPublisher,
                EventBusIntegrationEventPublisher>();
        }
    }
}
