using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Vendas.Infrastructure.Persistence;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Catalogo.Interfaces;
using Vendas.Infrastructure.Repositories;
using Vendas.Domain.Pedidos.Interfaces;

namespace Vendas.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructureApp(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SQLite");

            services.AddDbContext<VendasDbContext>(opt => opt.UseSqlite(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
        }
    }
}
