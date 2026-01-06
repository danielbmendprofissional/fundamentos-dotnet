using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Infrastructure.Persistence.Command.Mappings.Base;

namespace Vendas.Infrastructure.Persistence.Command.Mappings
{
    internal sealed class PagamentoMap : EntityMap<Pagamento>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");

            // FK técnica
            builder.Property<Guid>("PedidoId");

            builder.Property(p => p.MetodoPagamento)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.StatusPagamento)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Valor)
                .IsRequired();

            builder.Property(p => p.DataPagamento);

            builder.Property(p => p.CodigoTransacao)
                .HasMaxLength(100);

            // Índice útil para conciliação
            builder.HasIndex(p => p.CodigoTransacao);
        }
    }
}
