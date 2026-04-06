using Onion.API;
using Onion.API.Auth;
using Onion.Application;
using Onion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddSecurityInfrastructure(builder.Configuration)
    .AddAuthPolicies()
    .AddApplication()
    .AddPresentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseMiddleware<UsuarioMiddleware>();

app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();