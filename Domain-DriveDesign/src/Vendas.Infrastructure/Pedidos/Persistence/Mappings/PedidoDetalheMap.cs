using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Pedidos.Persistence.QueryModels;

namespace Vendas.Infrastructure.Pedidos.Persistence.Mappings
{
    internal sealed class PedidoDetalheMap
        : IEntityTypeConfiguration<PedidoDetalheModel>
    {
        public void Configure(EntityTypeBuilder<PedidoDetalheModel> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.NumeroPedido);
            builder.Property(p => p.ValorTotal);
            builder.Property(p => p.Status);
            builder.Property(p => p.DataCriacao);

            builder.Property(p => p.Cep)
                .HasColumnName("Cep");

            builder.Property(p => p.Cidade)
                .HasColumnName("Cidade");

            builder.Property(p => p.Estado)
                .HasColumnName("Estado");
        }
    }

}
