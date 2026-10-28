using Galo.Presentation.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Galo.Presentation.Extensions
{
    /// <summary>
    /// Padrão BFF (Backend For Frontend) com Keycloak:
    ///
    ///   navegador (localhost:4300) --/api--> proxy --> API (localhost:5205) --Authorization Code + PKCE--> Keycloak
    ///
    ///  - A API (cliente CONFIDENCIAL "estudos-api") faz o login OIDC e guarda a sessão (ticket no Redis).
    ///  - O navegador só recebe um cookie de sessão HttpOnly. Nunca vê client secret, access token nem refresh token.
    ///  - Sem fluxo implícito, sem password grant. PKCE (S256) ligado.
    ///  - O redirect do login é http://localhost:4300/api/v1/auth/callback (mesma origem do front, via proxy).
    /// </summary>
    public static class AuthenticationExtensions
    {
        public const string CallbackPath = "/api/v1/auth/callback";

        public static IServiceCollection AddGaloAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var origemFrontend = (configuration["Frontend:Origin"] ?? "http://localhost:4300").TrimEnd('/');
            var enforce = configuration.GetValue("Auth:Enforce", true);

            services.AddSingleton<ITicketStore, RedisTicketStore>();
            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore>((o, store) => o.SessionStore = store);

            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // 401 (e não redirect) quando a API exige login: quem decide ir ao Keycloak é o front, chamando /api/v1/auth/login
                    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(o =>
                {
                    o.Cookie.Name = "galo.sessao";
                    o.Cookie.HttpOnly = true;                         // JavaScript não lê o cookie (mitiga XSS)
                    o.Cookie.SameSite = SameSiteMode.Lax;             // não vai em POST cross-site (mitiga CSRF)
                    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // em dev é http; em produção use Always
                    o.ExpireTimeSpan = TimeSpan.FromHours(8);
                    o.SlidingExpiration = true;

                    // API não redireciona: devolve 401/403 para o front decidir (redirecionar para /api/v1/auth/login).
                    o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
                    o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
                {
                    var authority = configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak:Authority ausente");
                    var realm = configuration["Keycloak:Realm"] ?? throw new InvalidOperationException("Keycloak:Realm ausente");

                    // Aceita tanto "http://host:porta" quanto "http://host:porta/realms/estudos" na configuração.
                    o.Authority = authority.Contains("/realms/", StringComparison.OrdinalIgnoreCase)
                        ? authority.TrimEnd('/')
                        : $"{authority.TrimEnd('/')}/realms/{realm}";

                    o.ClientId = configuration["Keycloak:ClientId"];
                    o.ClientSecret = configuration["Keycloak:ClientSecret"];
                    o.RequireHttpsMetadata = o.Authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                    o.ResponseType = OpenIdConnectResponseType.Code;   // Authorization Code (nunca implicit)
                    o.UsePkce = true;                                  // PKCE com S256
                    o.ResponseMode = OpenIdConnectResponseMode.Query;  // o "code" volta na query string (GET): compatível com cookies SameSite=Lax

                    o.CallbackPath = CallbackPath;
                    o.SignedOutCallbackPath = "/api/v1/auth/signout-callback";
                    o.SignedOutRedirectUri = origemFrontend + "/";

                    o.Scope.Clear();
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("email");

                    o.SaveTokens = true;                               // tokens vão para o TICKET (que fica no Redis, não no navegador)
                    o.GetClaimsFromUserInfoEndpoint = true;
                    o.MapInboundClaims = false;                        // mantém nomes originais (preferred_username, email...)
                    o.TokenValidationParameters.NameClaimType = "preferred_username";

                    // Cookies temporários do handshake OIDC (correlation/nonce) também em Lax para funcionar em http.
                    o.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    o.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    o.NonceCookie.SameSite = SameSiteMode.Lax;
                    o.NonceCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                    o.Events = new OpenIdConnectEvents
                    {
                        // O navegador está em :4300 (proxy), mas a API enxerga o host :5205. O redirect_uri enviado ao
                        // Keycloak PRECISA ser exatamente o cadastrado: http://localhost:4300/api/v1/auth/callback
                        OnRedirectToIdentityProvider = ctx =>
                        {
                            var redirectUri = origemFrontend + CallbackPath;
                            ctx.ProtocolMessage.RedirectUri = redirectUri;
                            return Task.CompletedTask;
                        },
                        OnRedirectToIdentityProviderForSignOut = ctx =>
                        {
                            ctx.ProtocolMessage.PostLogoutRedirectUri = origemFrontend + "/";
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(o =>
            {
                // Por padrão TUDO exige login, exceto o que tiver [AllowAnonymous].
                // Em Development a flag Auth:Enforce=false libera tudo para estudar pelo Swagger sem fazer login.
                if (enforce) o.FallbackPolicy = o.DefaultPolicy;
            });

            return services;
        }
    }
}
