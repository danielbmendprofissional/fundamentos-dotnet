using Galo.Domain.Elenco.Entities;
using Galo.Domain.Elenco.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Galo.Infrastructure.Persistence.Mappings
{
    internal sealed class JogadorMap : EntityMap<Jogador>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Jogador> builder)
        {
            builder.ToTable("Jogadores");

            // Value Objects de 1 valor => VALUE CONVERTER: a coluna guarda o primitivo, a entidade enxerga o VO.
            builder.Property(j => j.Nome)
                .HasConversion(v => v.Valor, s => NomeJogador.Create(s))
                .HasMaxLength(100).IsRequired();

            builder.Property(j => j.Camisa)
                .HasConversion(v => v.Valor, n => NumeroCamisa.Create(n))
                .IsRequired();

            builder.Property(j => j.ValorMercado)
                .HasConversion(v => v.Valor, d => ValorMercado.Create(d))
                .HasColumnType("numeric(18,2)").IsRequired();

            builder.Property(j => j.Apelido).HasMaxLength(60);
            builder.Property(j => j.Nacionalidade).HasMaxLength(60).IsRequired();
            builder.Property(j => j.DataNascimento).IsRequired();

            // Enums como TEXTO: legível no banco e imune a reordenação do enum.
            builder.Property(j => j.Posicao).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(j => j.Status).HasConversion<string>().HasMaxLength(30).IsRequired();

            builder.HasIndex(j => j.Nome);
            builder.HasIndex(j => new { j.Posicao, j.Status });

            // Entidade filha: coleção privada (_contratos) acessada pelo CAMPO, para o EF não depender de setter público.
            builder.HasMany(j => j.Contratos).WithOne().HasForeignKey(c => c.JogadorId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(j => j.Contratos).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(j => j.DomainEvents);
        }
    }

    internal sealed class ContratoMap : EntityMap<Contrato>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Contrato> builder)
        {
            builder.ToTable("Contratos");

            builder.Property(c => c.Inicio).IsRequired();
            builder.Property(c => c.Fim).IsRequired();
            builder.Property(c => c.SalarioMensal).HasColumnType("numeric(18,2)").IsRequired();
            builder.Property(c => c.ClausulaRescisoria).HasColumnType("numeric(18,2)");

            builder.HasIndex(c => c.JogadorId);
        }
    }
}
