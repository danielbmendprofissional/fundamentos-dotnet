using MensageriaMQ.API.Relatorios;

namespace MensageriaMQ.API.Controllers
{
    internal static class ApiEndpoints
    {
        // Records são perfeitos para mensagens/eventos no RabbitMQ
        public record GreetingResponse(string Saudacao);

        public static void AddApiEndpoints(this WebApplication app)
        {
            app.MapPost("solicitar-relatorio/{name}", (string name) =>
            {
                var solicitacao = new SolicitacaoRelatorio
                {
                    Id = Guid.NewGuid(),
                    Nome = name,
                    Status = "Pendente",
                    ProcessedTime = null
                };

                Lista.Relatorios.Add(solicitacao);

                return Results.Ok();
            });

            app.MapGet("relatorios", () => Lista.Relatorios);
        }
    }
}
