using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Galo.Presentation.Infrastructure
{
    /// <summary>Verifica se o Keycloak publica o documento de descoberta OIDC (sem expor URL/credenciais na resposta).</summary>
    public sealed class KeycloakHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _configuration;

        public KeycloakHealthCheck(IHttpClientFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var authority = _configuration["Keycloak:Authority"];
                var realm = _configuration["Keycloak:Realm"];
                if (string.IsNullOrWhiteSpace(authority)) return HealthCheckResult.Unhealthy("Keycloak:Authority não configurado");

                var baseUrl = authority.Contains("/realms/", StringComparison.OrdinalIgnoreCase)
                    ? authority.TrimEnd('/')
                    : $"{authority.TrimEnd('/')}/realms/{realm}";

                var http = _factory.CreateClient("health");
                http.Timeout = TimeSpan.FromSeconds(5);
                using var resposta = await http.GetAsync($"{baseUrl}/.well-known/openid-configuration", cancellationToken);

                return resposta.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy("Keycloak respondendo")
                    : HealthCheckResult.Unhealthy($"Keycloak respondeu HTTP {(int)resposta.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Falha ao consultar o Keycloak", ex);
            }
        }
    }
}
