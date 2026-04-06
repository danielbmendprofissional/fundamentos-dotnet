using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Onion.Application.Common.Interfaces;
using Onion.Infrastructure.Persistence.Common.Context;
using Onion.Infrastructure.Persistence.Identity.Entities;
using System.Text;

namespace Onion.API.Auth
{
    public static class AuthServiceExtensions
    {
        public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(options => {
                options.Password.RequireDigit = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<CalendarUserContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                var secret = config["JWTConfigurationOptions:SecretKey"];
                var issue = config["JWTConfigurationOptions:Issuer"];     
                var audicence = config["JWTConfigurationOptions:Audience"];

                options.SaveToken = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer = issue,
                    ValidAudience = audicence,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userProvider = context.HttpContext.RequestServices.GetRequiredService<IUserProvider>();
                        var identityService = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();

                        var userId = userProvider.GetUserId();
                        var tokenVersionClaim = context.Principal?.FindFirst("TokenVersion")?.Value;

                        if (!await identityService.ValidateTokenVersionAsync(userId, int.Parse(tokenVersionClaim!)))
                        {
                            context.Fail("Este token foi invalidado por um logout ou troca de senha.");
                        }
                    }
                };
            })
            .AddGoogle(options => {
                options.ClientId = config["Authentication:Google:ClientId"]!;
                options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
            });

            return services;
        }

        public static IServiceCollection AddAuthPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}
