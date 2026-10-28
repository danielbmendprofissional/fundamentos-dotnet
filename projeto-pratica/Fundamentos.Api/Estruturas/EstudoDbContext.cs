using Microsoft.EntityFrameworkCore;

public class JogadorDb
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Posicao { get; set; } = "";
    public int Gols { get; set; }
}

public class EstudoDbContext(DbContextOptions<EstudoDbContext> options) : DbContext(options)
{
    public DbSet<JogadorDb> Jogadores => Set<JogadorDb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<JogadorDb>().HasIndex(j => j.Nome).IsUnique();
}
