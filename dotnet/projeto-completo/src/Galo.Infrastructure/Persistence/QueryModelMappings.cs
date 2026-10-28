using Galo.Infrastructure.Persistence.QueryModels;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence
{
    /// <summary>
    /// Mapeia os Query Models nas MESMAS tabelas do modelo de escrita. Usado por dois contextos:
    /// GaloQueryDbContext (produção, sem proxies) e EstudosDbContext (laboratório, com lazy loading).
    ///
    /// ATENÇÃO: aqui NÃO há migrations. O esquema pertence ao GaloDbContext (escrita). Se uma coluna mudar lá,
    /// ajuste aqui também.
    /// </summary>
    public static class QueryModelMappings
    {
        public static void Apply(ModelBuilder mb)
        {
            mb.Entity<JogadorQM>(b =>
            {
                b.ToTable("Jogadores");
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Contratos).WithOne(c => c.Jogador).HasForeignKey(c => c.JogadorId);
            });

            mb.Entity<ContratoQM>(b => { b.ToTable("Contratos"); b.HasKey(x => x.Id); });
            mb.Entity<ClubeQM>(b => { b.ToTable("Clubes"); b.HasKey(x => x.Id); });
            mb.Entity<EstadioQM>(b => { b.ToTable("Estadios"); b.HasKey(x => x.Id); });
            mb.Entity<CampeonatoQM>(b => { b.ToTable("Campeonatos"); b.HasKey(x => x.Id); });

            mb.Entity<PartidaQM>(b =>
            {
                b.ToTable("Partidas");
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Campeonato).WithMany().HasForeignKey(x => x.CampeonatoId);
                b.HasOne(x => x.Mandante).WithMany().HasForeignKey(x => x.MandanteId);
                b.HasOne(x => x.Visitante).WithMany().HasForeignKey(x => x.VisitanteId);
                b.HasOne(x => x.Estadio).WithMany().HasForeignKey(x => x.EstadioId);
                b.HasMany(x => x.Gols).WithOne(g => g.Partida).HasForeignKey(g => g.PartidaId);
                b.HasMany(x => x.Setores).WithOne().HasForeignKey(s => s.PartidaId);
            });

            mb.Entity<GolQM>(b =>
            {
                b.ToTable("Gols");
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Clube).WithMany().HasForeignKey(x => x.ClubeId);
                b.HasOne(x => x.Jogador).WithMany().HasForeignKey(x => x.JogadorId);
            });

            mb.Entity<SetorQM>(b => { b.ToTable("PartidaSetores"); b.HasKey(x => x.Id); });

            mb.Entity<TorcedorQM>(b =>
            {
                b.ToTable("Torcedores");
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Enderecos).WithOne().HasForeignKey(e => e.TorcedorId);
                b.HasMany(x => x.Compras).WithOne(c => c.Torcedor).HasForeignKey(c => c.TorcedorId);
            });

            mb.Entity<EnderecoQM>(b => { b.ToTable("Enderecos"); b.HasKey(x => x.Id); });

            mb.Entity<CompraQM>(b =>
            {
                b.ToTable("ComprasIngresso");
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Partida).WithMany().HasForeignKey(x => x.PartidaId);
                b.HasMany(x => x.Itens).WithOne().HasForeignKey(i => i.CompraId);
                b.HasMany(x => x.Pagamentos).WithOne().HasForeignKey(p => p.CompraId);
            });

            mb.Entity<ItemCompraQM>(b => { b.ToTable("ItensCompra"); b.HasKey(x => x.Id); });
            mb.Entity<PagamentoCompraQM>(b => { b.ToTable("PagamentosCompra"); b.HasKey(x => x.Id); });
        }
    }
}
