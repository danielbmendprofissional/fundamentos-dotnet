using Galo.Domain.Competicoes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Galo.Infrastructure.Persistence.Mappings
{
    internal sealed class ClubeMap : EntityMap<Clube>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Clube> builder)
        {
            builder.ToTable("Clubes");
            builder.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Sigla).HasMaxLength(3).IsRequired();
            builder.Property(c => c.Cidade).HasMaxLength(80).IsRequired();
            builder.Property(c => c.Uf).HasMaxLength(2).IsRequired();
            builder.Property(c => c.AnoFundacao).IsRequired();
            builder.Property(c => c.EhOGalo).IsRequired();

            builder.HasIndex(c => c.Sigla).IsUnique();
            builder.Ignore(c => c.DomainEvents);
        }
    }

    internal sealed class EstadioMap : EntityMap<Estadio>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Estadio> builder)
        {
            builder.ToTable("Estadios");
            builder.Property(e => e.Nome).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Cidade).HasMaxLength(80).IsRequired();
            builder.Property(e => e.Capacidade).IsRequired();
            builder.Ignore(e => e.DomainEvents);
        }
    }

    internal sealed class CampeonatoMap : EntityMap<Campeonato>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Campeonato> builder)
        {
            builder.ToTable("Campeonatos");
            builder.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Temporada).IsRequired();
            builder.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30).IsRequired();

            builder.HasIndex(c => c.Temporada);
            builder.Ignore(c => c.DomainEvents);
        }
    }

    internal sealed class PartidaMap : EntityMap<Partida>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Partida> builder)
        {
            builder.ToTable("Partidas");

            builder.Property(p => p.DataHoraUtc).IsRequired();
            builder.Property(p => p.Rodada).IsRequired();
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(30).IsRequired();

            // VO com 2 valores => OWNED TYPE: vira 2 colunas na própria tabela Partidas (GolsMandante/GolsVisitante).
            builder.OwnsOne(p => p.Placar, placar =>
            {
                placar.Property(x => x.GolsMandante).HasColumnName("GolsMandante").IsRequired();
                placar.Property(x => x.GolsVisitante).HasColumnName("GolsVisitante").IsRequired();
            });
            builder.Navigation(p => p.Placar).IsRequired();

            // Referências a OUTROS agregados: FK no banco (integridade), mas SEM propriedade de navegação no domínio.
            builder.HasOne<Campeonato>().WithMany().HasForeignKey(p => p.CampeonatoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Clube>().WithMany().HasForeignKey(p => p.MandanteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Clube>().WithMany().HasForeignKey(p => p.VisitanteId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Estadio>().WithMany().HasForeignKey(p => p.EstadioId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new { p.CampeonatoId, p.Rodada });
            builder.HasIndex(p => p.MandanteId);
            builder.HasIndex(p => p.VisitanteId);
            builder.HasIndex(p => p.DataHoraUtc);
            builder.HasIndex(p => p.Status);

            // Filhos do agregado
            builder.HasMany(p => p.Gols).WithOne().HasForeignKey(g => g.PartidaId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.Gols).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(p => p.Setores).WithOne().HasForeignKey(s => s.PartidaId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.Setores).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(p => p.IngressosDisponiveis);
            builder.Ignore(p => p.DomainEvents);
        }
    }

    internal sealed class GolPartidaMap : EntityMap<GolPartida>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<GolPartida> builder)
        {
            builder.ToTable("Gols");
            builder.Property(g => g.Minuto).IsRequired();
            builder.Property(g => g.Tipo).HasConversion<string>().HasMaxLength(30).IsRequired();

            builder.HasOne<Galo.Domain.Competicoes.Entities.Clube>().WithMany().HasForeignKey(g => g.ClubeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Galo.Domain.Elenco.Entities.Jogador>().WithMany().HasForeignKey(g => g.JogadorId).OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(g => g.PartidaId);
            builder.HasIndex(g => g.JogadorId);
        }
    }

    internal sealed class SetorPartidaMap : EntityMap<SetorPartida>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<SetorPartida> builder)
        {
            // A última barreira contra overselling: mesmo que o código falhe, o banco REJEITA estoque negativo.
            builder.ToTable("PartidaSetores", t =>
            {
                t.HasCheckConstraint("CK_PartidaSetores_Disponiveis", "\"Disponiveis\" >= 0 AND \"Disponiveis\" <= \"Capacidade\"");
            });

            builder.Property(s => s.Nome).HasMaxLength(60).IsRequired();
            builder.Property(s => s.Capacidade).IsRequired();
            builder.Property(s => s.Disponiveis).IsRequired();
            builder.Property(s => s.Preco).HasColumnType("numeric(18,2)").IsRequired();

            // CONCORRÊNCIA OTIMISTA com xmin: coluna de sistema do PostgreSQL (versão da linha, muda a cada UPDATE).
            // O EF gera "UPDATE ... WHERE Id = @id AND xmin = @xminLido". Se outro processo gravou antes,
            // 0 linhas são afetadas e o EF lança DbUpdateConcurrencyException (traduzida no UnitOfWork).
            builder.Property<uint>("xmin").IsRowVersion();

            builder.HasIndex(s => new { s.PartidaId, s.Nome }).IsUnique();
        }
    }
}
