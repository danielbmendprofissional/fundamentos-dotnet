using Onion.Application.Common.Interfaces;

namespace Onion.API.Auth
{
    public class UsuarioMiddleware
    {
        private readonly RequestDelegate _next;

        public UsuarioMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IUserProvider userProvider, IIdentityService identityService)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userId = userProvider.GetUserId();
            var tokenVersionClaim = context.User.FindFirst("TokenVersion")?.Value;

            if (int.TryParse(tokenVersionClaim, out int version))
            {
                var isTokenValid = await identityService.ValidateTokenVersionAsync(userId, version);

                if (!isTokenValid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "Sessão expirada. Faça login novamente." });
                    return;
                }
            }

            await _next(context);
        }
    }
}
