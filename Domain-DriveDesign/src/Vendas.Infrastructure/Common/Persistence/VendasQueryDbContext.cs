using Microsoft.EntityFrameworkCore;
using Vendas.Infrastructure.Clientes.Persistence.Mappings;
using Vendas.Infrastructure.Clientes.Persistence.QueryModels;
using Vendas.Infrastructure.Pedidos.Persistence.Mappings;
using Vendas.Infrastructure.Pedidos.Persistence.QueryModels;

namespace Vendas.Infrastructure.Common.Persistence
{
    public sealed class VendasQueryDbContext : DbContext
    {
        public DbSet<PedidoResumoModel> PedidoResumos => Set<PedidoResumoModel>();
        public DbSet<PedidoDetalheModel> PedidoDetalhes => Set<PedidoDetalheModel>();
        public DbSet<EnderecoPrincipalModel> EnderecoPrincipal => Set<EnderecoPrincipalModel>();
        public DbSet<ClienteIDsModel> ClientesIDs => Set<ClienteIDsModel>();

        public VendasQueryDbContext(DbContextOptions<VendasQueryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PedidoResumoMap());
            modelBuilder.ApplyConfiguration(new PedidoDetalheMap());
            modelBuilder.ApplyConfiguration(new EnderecoPrincipalMap());
            modelBuilder.ApplyConfiguration(new ClientesIDsMap());

            base.OnModelCreating(modelBuilder);
        }
    }

}
