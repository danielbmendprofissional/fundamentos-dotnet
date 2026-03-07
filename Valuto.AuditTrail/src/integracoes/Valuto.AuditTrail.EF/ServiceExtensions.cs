using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.EF.Adapters;
using Valuto.AuditTrail.Logic;
using Valuto.AuditTrail.Logic.Cache;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.EF;
using Valuto.AuditTrail.EF.Adapters;

namespace Valuto.AuditTrail.EF
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
        /// <param name="enviroment">Ambiente do nugget.</param>
        /// <param name="configure">Configurações opcionais do nugget.</param>
        /// <returns>A coleção de serviços para encadeamento de chamadas.</returns>
        public static IServiceCollection AddAuditTrail<TContext>(this IServiceCollection services,
            Action<AuditConfigurationOptions> configure = null)
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

            var descriptor = services.FirstOrDefault(d =>
                    d.ServiceType == typeof(TContext) ||
                    (d.ImplementationType == typeof(TContext)) ||
                    (d.ImplementationFactory != null && d.ServiceType.IsAssignableFrom(typeof(TContext))))
                ?? throw new InvalidOperationException($"DbContext {typeof(TContext).Name} não está acessível no DI. ");

            services.Replace(ServiceDescriptor.Describe(
                descriptor.ServiceType,
                sp => {
                    TContext contexto;
                    if (descriptor.ImplementationFactory != null)
                    {
                        contexto = (TContext)descriptor.ImplementationFactory(sp);
                    }
                    else if (descriptor.ImplementationType != null)
                    {
                        contexto = (TContext)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.ImplementationType);
                    }
                    else
                    {
                        contexto = sp.GetRequiredService<TContext>();
                    }
                    var auditoria = sp.GetRequiredService<IAuditoriaInterna>();
                    contexto.AuditoriaInterna = auditoria;

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
