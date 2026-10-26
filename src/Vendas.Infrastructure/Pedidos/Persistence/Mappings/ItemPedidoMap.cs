using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Infrastructure.Common.Persistence.Mappings;

namespace Vendas.Infrastructure.Pedidos.Persistence.Mappings
{
    internal sealed class ItemPedidoMap : EntityMap<ItemPedido>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.ToTable("ItensPedido");

            // FK técnica
            builder.Property<Guid>("PedidoId");

            builder.Property(i => i.ProdutoId)
                .IsRequired();

            builder.Property(i => i.NomeProduto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.PrecoUnitario)
                .IsRequired();

            builder.Property(i => i.Quantidade)
                .IsRequired();

            builder.Property(i => i.PorcentagemDesconto)
                .IsRequired();

            builder.Property(i => i.ValorTotal)
                .IsRequired();

            // Índice técnico (exemplo comum)
            builder.HasIndex("PedidoId", nameof(ItemPedido.ProdutoId))
                .IsUnique();
        }
    }
}
