using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Galo.Infrastructure.Persistence
{
    /// <summary>
    /// Usado SÓ pelas ferramentas "dotnet ef" (design-time): elas não sobem o Program.cs, então precisam saber
    /// como construir o contexto. Lê a connection string de MIGRATIONS (usuário "owner", com DDL) dos user-secrets.
    ///
    /// Para só GERAR uma migration (migrations add) não há conexão com o banco: se o secret não existir usamos um
    /// placeholder sem credenciais. Para APLICAR (database update) o secret é obrigatório.
    /// </summary>
    internal sealed class GaloDbContextFactory : IDesignTimeDbContextFactory<GaloDbContext>
    {
        // mesmo UserSecretsId do Galo.Presentation.csproj
        private const string UserSecretsId = "68a1a61a-252c-4d61-83d2-2b9ef1736df6";

        public GaloDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(UserSecretsId)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("PostgresMigrations")
                                   ?? "Host=localhost;Database=estudos"; // placeholder: nunca é usado para conectar

            var options = new DbContextOptionsBuilder<GaloDbContext>()
                .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(GaloDbContext).Assembly.FullName))
                .Options;

            return new GaloDbContext(options);
        }
    }
}
