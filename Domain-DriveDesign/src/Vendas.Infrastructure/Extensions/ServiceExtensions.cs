using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vendas.Application.Abstractions.IntegrationEvents;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Application.Abstractions.Persistence.Queries;
using Vendas.Domain.Catalogo.Interfaces;
using Vendas.Domain.Pedidos.Interfaces;
using Vendas.Infrastructure.IntegrationEventHandlers;
using Vendas.Infrastructure.Persistence.Command;
using Vendas.Infrastructure.Persistence.Query;
using Vendas.Infrastructure.Repositories.Command;
using Vendas.Infrastructure.Repositories.Queries;

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
