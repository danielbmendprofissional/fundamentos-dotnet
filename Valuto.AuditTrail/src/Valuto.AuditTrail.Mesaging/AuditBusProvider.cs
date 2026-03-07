using MassTransit;
using Microsoft.Extensions.Options;
using System;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Mesaging.Interfaces;
using Valuto.AuditTrail.Mesaging.Options;
using Valuto.AuditTrail.Mesaging.Interfaces;

using Valuto.AuditTrail.Abstractions.Model.Events;


#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1 || NET6_0 || NET7_0
using GreenPipes;
#endif

namespace Valuto.AuditTrail.Mesaging
{
    internal class AuditBusProvider : IAuditBusProvider, IDisposable
    {
        private readonly IBusControl _busControl;
        private bool _isStarted;

        public AuditBusProvider(IAuditOptions options)
        {
            var opt = options;

            _busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1
                cfg.Host(new Uri($"rabbitmq://{opt.Host}"), h =>
                {
                    h.Username(opt.Username);
                    h.Password(opt.Password);
                });
#else
                cfg.Host(opt.Host, h =>
                {
                    h.Username(opt.Username);
                    h.Password(opt.Password);
                });
#endif

                cfg.Message<AuditTrailCreatedEvent>(m => m.SetEntityName(options.Exchange));
                cfg.Publish<AuditTrailCreatedEvent>(x =>
                {
                    x.BindQueue(options.Exchange, options.Queue);
                });

                cfg.Message<AuditLogEvent>(m =>
                {
                    m.SetEntityName(options.LogExchange);
                });

                cfg.Publish<AuditLogEvent>(x =>
                {
                    x.BindQueue(options.LogExchange, options.LogQueue);
                });

#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1
                cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(3)));
#endif

#if NET6_0 || NET7_0
                cfg.UseMessageRetry(r => 
                {
                    r.Handle<Exception>(); 

                    r.Exponential(5,
                                  TimeSpan.FromSeconds(1),
                                  TimeSpan.FromSeconds(30),
                                  TimeSpan.FromSeconds(2));
                });
#endif

#if NET8_0
                cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(2)));
#endif

            });
        }

        public IBusControl GetBus()
        {
            return _busControl;
        }

        public void Dispose() => _busControl?.Stop(TimeSpan.FromSeconds(5));
    }
}