using Galo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Galo.Estudos.Persistence
{
    /// <summary>
    /// DbContext do LABORATÓRIO: reaproveita os Query Models (mesmas tabelas), mas é configurado com
    /// UseLazyLoadingProxies() (ver AddEstudos). Com os proxies, acessar uma navegação ainda não carregada
    /// (partida.Mandante) dispara um SELECT automaticamente - perfeito para demonstrar o problema N+1.
    ///
    /// ATENÇÃO: lazy loading é uma escolha controversa. Ele "funciona sem você pedir", o que esconde dezenas de
    /// consultas. Em APIs, o recomendado é Include (eager) ou projeção (Select).
    /// Por padrão este contexto RASTREIA entidades (QueryTrackingBehavior.TrackAll), ao contrário do GaloQueryDbContext.
    /// </summary>
    public sealed class EstudosDbContext : DbContext
    {
        public EstudosDbContext(DbContextOptions<EstudosDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            QueryModelMappings.Apply(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
