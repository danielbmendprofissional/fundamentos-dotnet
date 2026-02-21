using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Valuto.AuditTrail.Worker.Consumers;
using Valuto.AuditTrail.Worker.Interfaces;
using Valuto.AuditTrail.Worker.Persistence;
using Valuto.AuditTrail.Worker.Persistence.Repository;


namespace Valuto.AuditTrail.Worker.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuditWorkerInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var cnnstr = configuration.GetConnectionString("default");

            services.AddDbContext<AuditDbContext>(opt =>
            {
                opt.UseOracle(cnnstr);
                opt.LogTo(s => System.Diagnostics.Debug.WriteLine(s)).EnableDetailedErrors();
                opt.EnableSensitiveDataLogging();
            });

            services.AddScoped<IAuditTrailsRepository, AuditTrailsRepository>();
            services.AddScoped<IAuditLogsRepository, AuditLogsRepository>();

            services.Configure<RabbitOptions>(configuration.GetSection("RabbitOptions"));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<AuditTrailConsumer>();
                x.AddConsumer<AuditLogsConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitOptions>>().Value;

                    cfg.Host(options.Host, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint(options.Queue, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 10;
                            cb.ActiveThreshold = 5;
                            cb.ResetInterval = TimeSpan.FromSeconds(30);
                        });

                        //Comentado até o potter instalar o plugin no rabbit para que isso funcione...
                        //plugin: rabbitmq-delayed-message-exchange

                        //e.UseScheduledRedelivery(r =>
                        //{
                        //    r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30), TimeSpan.FromHours(2));
                        //});

                        e.Bind(options.Exchange);
                        e.ConfigureConsumer<AuditTrailConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(options.LogQueue, e =>
                    {
                        e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(10)));

                        e.Bind(options.LogExchange);
                        e.ConfigureConsumer<AuditLogsConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
