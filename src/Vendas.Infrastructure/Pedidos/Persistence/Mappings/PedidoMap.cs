using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Infrastructure.Common.Persistence.Mappings;

namespace Vendas.Infrastructure.Pedidos.Persistence.Mappings
{
    internal sealed class PedidoMap : EntityMap<Pedido>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.Property(p => p.ClienteId)
                .IsRequired();

            builder.Property(p => p.NumeroPedido)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.StatusPedido)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.ValorTotal)
                .IsRequired();

            // ===============================
            // EnderecoEntrega (Value Object)
            // ===============================
            builder.OwnsOne(p => p.EnderecoEntrega, endereco =>
            {
                endereco.OwnsOne(e => e.Cep, cep =>
                {
                    cep.Property(c => c.Valor)
                        .HasColumnName("Cep")
                        .IsRequired()
                        .HasMaxLength(8);
                });

                endereco.Property(e => e.Logradouro)
                    .IsRequired()
                    .HasMaxLength(200);

                endereco.Property(e => e.Complemento)
                    .HasMaxLength(200);

                endereco.Property(e => e.Bairro)
                    .IsRequired()
                    .HasMaxLength(100);

                endereco.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(2);

                endereco.Property(e => e.Cidade)
                    .IsRequired()
                    .HasMaxLength(100);

                endereco.Property(e => e.Pais)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // ===============================
            // Relacionamentos (campos)
            // ===============================
            builder.HasMany("_itens")
                .WithOne()
                .HasForeignKey("PedidoId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany("_pagamentos")
                .WithOne()
                .HasForeignKey("PedidoId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata
                .FindNavigation("_itens")!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata
                .FindNavigation("_pagamentos")!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
