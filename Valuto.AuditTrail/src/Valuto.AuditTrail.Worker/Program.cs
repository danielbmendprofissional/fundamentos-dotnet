using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valuto.AuditTrail.Worker.Extensions;
using Valuto.AuditTrail.Worker.Persistence;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddAuditWorkerInfrastructure(context.Configuration);
    })
    .Build();

await host.RunAsync();