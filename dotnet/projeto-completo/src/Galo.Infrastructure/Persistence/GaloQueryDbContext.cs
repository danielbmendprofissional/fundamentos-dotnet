using Galo.Infrastructure.Persistence.QueryModels;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext de LEITURA (lado "Query" do CQRS). Nunca grava: por padrão NÃO rastreia entidades (NoTracking),
    /// o que economiza memória/CPU em listagens (sem snapshot, sem identity map).
    /// </summary>
    public sealed class GaloQueryDbContext : DbContext
    {
        public DbSet<JogadorQM> Jogadores => Set<JogadorQM>();
        public DbSet<ContratoQM> Contratos => Set<ContratoQM>();
        public DbSet<ClubeQM> Clubes => Set<ClubeQM>();
        public DbSet<EstadioQM> Estadios => Set<EstadioQM>();
        public DbSet<CampeonatoQM> Campeonatos => Set<CampeonatoQM>();
        public DbSet<PartidaQM> Partidas => Set<PartidaQM>();
        public DbSet<GolQM> Gols => Set<GolQM>();
        public DbSet<SetorQM> Setores => Set<SetorQM>();
        public DbSet<TorcedorQM> Torcedores => Set<TorcedorQM>();
        public DbSet<EnderecoQM> Enderecos => Set<EnderecoQM>();
        public DbSet<CompraQM> Compras => Set<CompraQM>();

        public GaloQueryDbContext(DbContextOptions<GaloQueryDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            QueryModelMappings.Apply(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges() => throw new InvalidOperationException("GaloQueryDbContext é somente leitura.");

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("GaloQueryDbContext é somente leitura.");
    }
}
