using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Interfaces.Repository;
using CleanArchitecture.Domain.Interfaces.Repository.Base;
using CleanArchitecture.Infrastructure.AntiCorruption;
using CleanArchitecture.Infrastructure.AntiCorruption.Options;
using CleanArchitecture.Infrastructure.Persistence.Context;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    /*
        Extension method ConfigurePersistenceApp for IServiceCollection
    */
    public static class ServiceExtensions
    {
        public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
        {
            var cnnstr = configuration.GetConnectionString("DefaultConnection");
            var schema = configuration["DatabaseConfigurationOptions:DefaultSchema"];
            services.AddDbContext<AppDbContext>(x =>
            {
                x.UseNpgsql(cnnstr, c =>
                {
                    c.MigrationsHistoryTable("historico_migracoes", schema);
                    c.MigrationsAssembly("CleanArchitecture.Infrastructure");
                });
                x.LogTo(s => System.Diagnostics.Debug.WriteLine(s)).EnableDetailedErrors();
                x.EnableSensitiveDataLogging();
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEmailService, EmailService>();
            services.Configure<EmailConfigurationOptions>(configuration.GetSection(nameof(EmailConfigurationOptions)));
        }
    }
}
