using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Persistence.Query.Models;

namespace Vendas.Infrastructure.Persistence.Query
{
    public sealed class VendasQueryDbContext : DbContext
    {
        public DbSet<PedidoResumoQueryModel> PedidoResumos => Set<PedidoResumoQueryModel>();
        public DbSet<PedidoDetalheQueryModel> PedidoDetalhes => Set<PedidoDetalheQueryModel>();

        public VendasQueryDbContext(DbContextOptions<VendasQueryDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(VendasQueryDbContext).Assembly);
        }
    }

}
