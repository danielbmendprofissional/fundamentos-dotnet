using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Galo.Presentation.Controllers
{
    /// <summary>
    /// Endpoints do BFF. O front só navega para /login e consulta /me; não sabe nada de tokens.
    /// (O callback /api/v1/auth/callback é tratado pelo middleware do OpenID Connect, não por uma action.)
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration) => _configuration = configuration;

        /// <summary>Inicia o login (redireciona o navegador para o Keycloak).</summary>
        [HttpGet("login"), AllowAnonymous]
        public IActionResult Login([FromQuery] string? returnUrl = "/")
        {
            var origem = (_configuration["Frontend:Origin"] ?? "http://localhost:4300").TrimEnd('/');

            // Anti open-redirect: só aceita caminho LOCAL ("/algo"), nunca URL externa nem "//host".
            var destino = !string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//")
                ? returnUrl
                : "/";

            return Challenge(new AuthenticationProperties { RedirectUri = origem + destino }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>Quem sou eu? 401 se não houver sessão. Nunca devolve tokens.</summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me() => Ok(new
        {
            autenticado = true,
            usuario = User.Identity?.Name,
            email = User.FindFirst("email")?.Value,
            nome = User.FindFirst("name")?.Value
        });

        /// <summary>Encerra a sessão local (Redis) e a sessão no Keycloak (single logout).</summary>
        [AllowAnonymous]
        [HttpGet("logout")]
        [HttpPost("logout")]
        public IActionResult Logout()
            => SignOut(new AuthenticationProperties(),
                CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
