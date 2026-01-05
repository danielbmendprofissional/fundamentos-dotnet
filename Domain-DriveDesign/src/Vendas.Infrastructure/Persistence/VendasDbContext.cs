using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Infrastructure.Persistence
{
    public sealed class VendasDbContext : DbContext
    {
        // Expor SOMENTE Aggregate Roots
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Produto> Produtos => Set<Produto>();

        public VendasDbContext(DbContextOptions<VendasDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todos os mappings (PedidoMap, ItemPedidoMap, PagamentoMap, etc)
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(VendasDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {

            var domainEvents = ChangeTracker
                .Entries<Entity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // Limpa eventos após persistência
            foreach (var entry in ChangeTracker.Entries<Entity>())
                entry.Entity.ClearDomainEvents();

            // Dispatcher / Outbox entram depois
            // foreach (var domainEvent in domainEvents)
            //     await _dispatcher.Dispatch(domainEvent);

            return result;
        }
    }
}
