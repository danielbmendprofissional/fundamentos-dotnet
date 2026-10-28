using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISocioRepository, SocioRepositoryEmMemoria>();
builder.Services.AddMemoryCache();

var conexao = new SqliteConnection("DataSource=:memory:");
conexao.Open();                                       // o banco em memória morre quando a conexão fecha
builder.Services.AddSingleton(conexao);
builder.Services.AddDbContext<EstudoDbContext>((sp, o) => o.UseSqlite(sp.GetRequiredService<SqliteConnection>()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EstudoDbContext>();
    db.Database.EnsureCreated();

    // um único INSERT ... SELECT em vez de 50 mil INSERTs (e 50 mil linhas de log)
    db.Database.ExecuteSqlRaw("""
        WITH RECURSIVE seq(i) AS (SELECT 1 UNION ALL SELECT i + 1 FROM seq WHERE i < 50000)
        INSERT INTO "Jogadores" ("Nome", "Posicao", "Gols")
        SELECT 'Jogador ' || i,
               CASE i % 4 WHEN 0 THEN 'Atacante' WHEN 1 THEN 'Meia' WHEN 2 THEN 'Zagueiro' ELSE 'Goleiro' END,
               i % 30
        FROM seq;
        """);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
