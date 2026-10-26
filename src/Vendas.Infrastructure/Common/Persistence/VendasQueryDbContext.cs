using Microsoft.EntityFrameworkCore;
using Vendas.Infrastructure.Catalogo.Categorias.Persistence.Mappings;
using Vendas.Infrastructure.Catalogo.Categorias.Persistence.QueryModels;
using Vendas.Infrastructure.Catalogo.Produtos.Persistence.Mappings;
using Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels;
using Vendas.Infrastructure.Clientes.Persistence.Mappings;
using Vendas.Infrastructure.Clientes.Persistence.QueryModels;
using Vendas.Infrastructure.Pedidos.Persistence.Mappings;
using Vendas.Infrastructure.Pedidos.Persistence.QueryModels;

namespace Vendas.Infrastructure.Common.Persistence
{
    public sealed class VendasQueryDbContext : DbContext
    {
        public DbSet<PedidoResumoModel> PedidoResumo => Set<PedidoResumoModel>();
        public DbSet<PedidoDetalheModel> PedidoDetalhes => Set<PedidoDetalheModel>();
        public DbSet<EnderecoPrincipalModel> EnderecoPrincipal => Set<EnderecoPrincipalModel>();
        public DbSet<ClienteIDsModel> ClientesIDs => Set<ClienteIDsModel>();
        public DbSet<CategoriaResumoModel> CategoriaResumo => Set<CategoriaResumoModel>();
        public DbSet<CategoriaDetalheModel> CategoriaDetalhes => Set<CategoriaDetalheModel>();
        public DbSet<ProdutoResumoModel> ProdutoResumo => Set<ProdutoResumoModel>();
        public DbSet<ProdutoDetalhesModel> ProdutoDetalhes => Set<ProdutoDetalhesModel>();
        public DbSet<ImagemProdutoModel> ImagemProduto => Set<ImagemProdutoModel>();

        public VendasQueryDbContext(DbContextOptions<VendasQueryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PedidoResumoMap());
            modelBuilder.ApplyConfiguration(new PedidoDetalheMap());
            modelBuilder.ApplyConfiguration(new EnderecoPrincipalMap());
            modelBuilder.ApplyConfiguration(new ClientesIDsMap());
            modelBuilder.ApplyConfiguration(new CategoriaResumoMap());
            modelBuilder.ApplyConfiguration(new CategoriaDetalheMap());
            modelBuilder.ApplyConfiguration(new ProdutoResumoMap());
            modelBuilder.ApplyConfiguration(new ProdutoDetalhesMap());
            modelBuilder.ApplyConfiguration(new ImagemProdutoMap());

            base.OnModelCreating(modelBuilder);
        }
    }

}
