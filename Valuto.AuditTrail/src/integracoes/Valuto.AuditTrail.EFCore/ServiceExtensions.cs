using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.EFCore.Adapters;
using Valuto.AuditTrail.Logic;
using Valuto.AuditTrail.Logic.Cache;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.EFCore.Adapters;
using Valuto.AuditTrail.Logic;

namespace Valuto.AuditTrail.EFCore
{
    ///<summary>
    /// Providencia métodos de extensão para configuração da auditoria do Valuto.
    ///</summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adiciona os serviços necessários para o funcionamento da trilha de auditoria.
        /// </summary>
        /// <param name="services">A coleção de serviços do container de DI.</param>
        /// /// <param name="configure">Configurações opcionais do nugget.</param>
        /// <returns>A coleção de serviços para encadeamento de chamadas.</returns>
        public static IServiceCollection AddAuditTrail<TContext>(this IServiceCollection services,
        Action<AuditConfigurationOptions>? configure = null)
            where TContext : ValutoDbContext<TContext>
        {
            var options = new AuditConfigurationOptions();
            configure?.Invoke(options);

            if (options.SelectedEnviroment == 0)
            {
                throw new InvalidOperationException("É obrigatório configurar o ambiente via options.ConfigureEnviroment(Enviroments.Producao/Homologacao).");
            }

            services.AddSingleton(options);

            services.AddAuditLogic(options.SelectedEnviroment);

            services.RegisterAuditEngine<AuditTrailCreatedEvent>();

            services.RegisterAuditEngine<AuditLogEvent>();

            services.AddTransient<IAuditEntry, AuditEntryAdapter>();
            services.AddTransient<IAuditProperty, AuditPropertyAdapter>();
            services.AddScoped<IAuditTracker, AuditTracker>();

            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TContext));
            if (descriptor == null) throw new InvalidOperationException($"DbContext {typeof(TContext).Name} não registrado.");

            services.Replace(ServiceDescriptor.Describe(
                typeof(TContext),
                sp => {
                    var contexto = ActivatorUtilities.CreateInstance<TContext>(sp);
                    contexto.AuditoriaInterna = sp.GetRequiredService<IAuditoriaInterna>();

                    return contexto;
                },
                descriptor.Lifetime));

            services.AddSingleton<EfMetadataAdapter>();
            services.AddSingleton<IEfMetadataAdapter>(sp =>
            {
                var realService = sp.GetRequiredService<EfMetadataAdapter>();
                return new EfMetadataCacheProxy(realService);
            });

            return services;
        }
    }
}
