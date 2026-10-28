using System.Text.Json.Serialization;
using Galo.Application.Extensions;
using Galo.Estudos.Extensions;
using Galo.Infrastructure.Extensions;
using Galo.Infrastructure.Persistence;
using Galo.Presentation.Extensions;
using Galo.Presentation.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Em Development o host padrão carrega os user-secrets em builder.Configuration
// (ConnectionStrings:Postgres, Redis, RabbitMQ:*, Keycloak:*, OTEL_*). Nada de segredo em appsettings.

// ---- Camadas ----
builder.Services.ConfigureApplicationApp();
builder.Services.ConfigureInfrastructureApp(builder.Configuration);
builder.Services.AddEstudos(builder.Configuration);   // laboratório de estudos (EF, paralelismo, cache, concorrência...)

// ---- Observabilidade, autenticação e saúde ----
builder.Services.AddGaloObservability(builder.Configuration);
builder.Services.AddGaloAuthentication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddGaloHealthChecks().AddCheck<KeycloakHealthCheck>("keycloak", tags: new[] { "auth" });

// ---- Web API ----
builder.Services.AddControllers().AddJsonOptions(o =>
{
    // enums na API como texto ("Atacante") e não como número (6)
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GaloExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // O schema é criado pelas MIGRATIONS usando a conexão "owner" (a conexão de runtime não tem DDL).
    if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
        await DatabaseMigrator.MigrarAsync(app.Configuration, app.Logger);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// /health: monitoração. Anônimo, e responde só o status agregado (sem detalhes sensíveis).
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            dependencias = report.Entries.ToDictionary(e => e.Key, e => e.Value.Status.ToString())
        });
    },
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).AllowAnonymous();

// /metrics: formato Prometheus (scrape da monitoração na porta 5205). Anônimo: o Prometheus não faz login.
app.MapPrometheusScrapingEndpoint("/metrics").AllowAnonymous();

await app.RunAsync();

// necessário para testes de integração (WebApplicationFactory<Program>)
public partial class Program;
