using Galo.Domain.Common.Base;
using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Elenco.Entities;
using Galo.Domain.Ingressos.Entities;
using Galo.Domain.Torcida.Entities;
using Galo.Infrastructure.Persistence.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Galo.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext de ESCRITA (lado "Command" do CQRS): expõe SOMENTE Aggregate Roots.
    /// Filhos (Contrato, Gol, Setor, Endereco, Item, Pagamento) são alcançados pela raiz.
    ///
    /// Também é a nossa UNIT OF WORK: o ChangeTracker acumula as mudanças e SaveChangesAsync grava tudo numa transação.
    /// </summary>
    public sealed class GaloDbContext : DbContext
    {
        public DbSet<Jogador> Jogadores => Set<Jogador>();
        public DbSet<Clube> Clubes => Set<Clube>();
        public DbSet<Estadio> Estadios => Set<Estadio>();
        public DbSet<Campeonato> Campeonatos => Set<Campeonato>();
        public DbSet<Partida> Partidas => Set<Partida>();
        public DbSet<Torcedor> Torcedores => Set<Torcedor>();
        public DbSet<CompraIngresso> Compras => Set<CompraIngresso>();

        private readonly IPublisher? _publisher;

        // IPublisher é opcional: em design-time (dotnet ef) não existe DI de aplicação.
        public GaloDbContext(DbContextOptions<GaloDbContext> options, IPublisher? publisher = null) : base(options)
        {
            _publisher = publisher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new JogadorMap());
            modelBuilder.ApplyConfiguration(new ContratoMap());
            modelBuilder.ApplyConfiguration(new ClubeMap());
            modelBuilder.ApplyConfiguration(new EstadioMap());
            modelBuilder.ApplyConfiguration(new CampeonatoMap());
            modelBuilder.ApplyConfiguration(new PartidaMap());
            modelBuilder.ApplyConfiguration(new GolPartidaMap());
            modelBuilder.ApplyConfiguration(new SetorPartidaMap());
            modelBuilder.ApplyConfiguration(new TorcedorMap());
            modelBuilder.ApplyConfiguration(new EnderecoMap());
            modelBuilder.ApplyConfiguration(new CompraIngressoMap());
            modelBuilder.ApplyConfiguration(new ItemIngressoMap());
            modelBuilder.ApplyConfiguration(new PagamentoIngressoMap());

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1) coleta os agregados que registraram eventos de domínio (antes de gravar)
            var agregadosComEventos = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Count > 0)
                .Select(e => e.Entity)
                .ToList();

            var eventos = agregadosComEventos.SelectMany(a => a.DomainEvents).ToList();

            // 2) grava TUDO numa transação (se falhar aqui, nenhum evento é publicado)
            var linhas = await base.SaveChangesAsync(cancellationToken);

            // 3) limpa ANTES de publicar: se um handler chamar SaveChanges de novo, não republica os mesmos eventos
            agregadosComEventos.ForEach(a => a.ClearDomainEvents());

            // 4) publica DEPOIS do commit (os handlers enxergam o dado já gravado)
            //    Limitação conhecida: se o processo cair entre 2 e 4, o evento se perde.
            //    A solução profissional é o padrão OUTBOX (gravar o evento na mesma transação e despachar depois).
            if (_publisher is not null)
                foreach (var evento in eventos)
                    await _publisher.Publish(evento, cancellationToken);

            return linhas;
        }
    }
}
