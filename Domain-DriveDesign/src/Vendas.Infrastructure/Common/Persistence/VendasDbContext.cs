using MediatR;
using Microsoft.EntityFrameworkCore;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Infrastructure.Catalogo.Persistence.Mappings;
using Vendas.Infrastructure.Clientes.Persistence.Mappings;
using Vendas.Infrastructure.Pedidos.Persistence.Mappings;

namespace Vendas.Infrastructure.Common.Persistence
{
    public sealed class VendasDbContext : DbContext
    {
        // Expor SOMENTE Aggregate Roots
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Categoria> Categorias => Set<Categoria>();
        public DbSet<Produto> Produtos => Set<Produto>();

        private readonly IMediator _mediator;

        public VendasDbContext(DbContextOptions<VendasDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todos os mappings (PedidoMap, ItemPedidoMap, PagamentoMap, etc)
            modelBuilder.ApplyConfiguration(new CategoriaMap());
            modelBuilder.ApplyConfiguration(new ProdutoMap());
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new EnderecoMap());
            modelBuilder.ApplyConfiguration(new ItemPedidoMap());
            modelBuilder.ApplyConfiguration(new PagamentoMap());
            modelBuilder.ApplyConfiguration(new PedidoMap());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var entitiesWithEvents = ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // publish eventos registrados pelo Domínio
            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);

            // clear
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            return result;

            //IDEAL aplicar Outbox

        }
    }
}
