using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Galo.Infrastructure.Persistence
{
    /// <summary>
    /// Aplica as migrations usando a connection string de MIGRATIONS (usuário owner).
    ///
    /// Separação de privilégios (princípio do menor privilégio):
    ///   - ConnectionStrings:PostgresMigrations (owner) => pode CRIAR/ALTERAR tabelas. Usada SÓ aqui e no "dotnet ef".
    ///   - ConnectionStrings:Postgres (app)             => só SELECT/INSERT/UPDATE/DELETE. Usada pela API em runtime.
    /// Assim, uma SQL injection ou um bug na API não consegue dar DROP TABLE.
    /// </summary>
    public static class DatabaseMigrator
    {
        public static async Task MigrarAsync(IConfiguration configuration, ILogger logger, CancellationToken cancellationToken = default)
        {
            var connectionString = configuration.GetConnectionString("PostgresMigrations")
                ?? throw new InvalidOperationException("Chave ConnectionStrings:PostgresMigrations não encontrada (user-secrets).");

            var options = new DbContextOptionsBuilder<GaloDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using var contexto = new GaloDbContext(options); // sem IPublisher: migration não dispara eventos

            var pendentes = (await contexto.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pendentes.Count == 0)
            {
                logger.LogInformation("Banco já está atualizado (nenhuma migration pendente)");
                return;
            }

            logger.LogInformation("Aplicando {Qtd} migration(s): {Nomes}", pendentes.Count, string.Join(", ", pendentes));
            await contexto.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Migrations aplicadas");
        }
    }
}
