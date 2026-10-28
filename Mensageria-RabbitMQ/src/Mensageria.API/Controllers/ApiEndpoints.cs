namespace Mensageria.API.Controllers
{
    internal static class ApiEndpoints
    {
        // Records são perfeitos para mensagens/eventos no RabbitMQ
        public record GreetingResponse(string Saudacao);

        public static void AddApiEndpoints(this WebApplication app)
        {
            app.MapGet("hello-world", () => new GreetingResponse("hello world!"));
        }
    }
}
