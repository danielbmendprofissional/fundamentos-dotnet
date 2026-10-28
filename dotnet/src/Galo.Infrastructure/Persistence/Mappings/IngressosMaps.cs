using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Ingressos.Entities;
using Galo.Domain.Ingressos.ValueObjects;
using Galo.Domain.Torcida.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Galo.Infrastructure.Persistence.Mappings
{
    internal sealed class CompraIngressoMap : EntityMap<CompraIngresso>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<CompraIngresso> builder)
        {
            builder.ToTable("ComprasIngresso");

            builder.Property(c => c.Numero).HasMaxLength(20).IsRequired();
            builder.Property(c => c.ValorTotal).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(c => c.ReservaConfirmada).IsRequired();

            builder.Property(c => c.MotivoCancelamento)
                .HasConversion(v => v!.Codigo, s => MotivoCancelamento.De(s))
                .HasMaxLength(40);

            builder.HasOne<Torcedor>().WithMany().HasForeignKey(c => c.TorcedorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Partida>().WithMany().HasForeignKey(c => c.PartidaId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Numero).IsUnique();
            builder.HasIndex(c => c.TorcedorId);
            builder.HasIndex(c => c.PartidaId);
            builder.HasIndex(c => c.Status);

            builder.HasMany(c => c.Itens).WithOne().HasForeignKey(i => i.CompraId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(c => c.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(c => c.Pagamentos).WithOne().HasForeignKey(p => p.CompraId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(c => c.Pagamentos).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(c => c.TotalDeIngressos);
            builder.Ignore(c => c.DomainEvents);
        }
    }

    internal sealed class ItemIngressoMap : EntityMap<ItemIngresso>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<ItemIngresso> builder)
        {
            builder.ToTable("ItensCompra");
            builder.Property(i => i.Setor).HasMaxLength(60).IsRequired();
            builder.Property(i => i.PrecoUnitario).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(i => i.Quantidade).IsRequired();
            builder.Property(i => i.PorcentagemDesconto).HasColumnType("numeric(5,2)").IsRequired();
            builder.Property(i => i.ValorTotal).HasColumnType("numeric(18,2)").IsRequired();

            builder.HasIndex(i => i.CompraId);
        }
    }

    internal sealed class PagamentoIngressoMap : EntityMap<PagamentoIngresso>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<PagamentoIngresso> builder)
        {
            builder.ToTable("PagamentosCompra");
            builder.Property(p => p.Metodo).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(p => p.Valor).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(p => p.DataPagamento);
            builder.Property(p => p.CodigoTransacao).HasMaxLength(40);

            builder.HasIndex(p => p.CompraId);
        }
    }
}
