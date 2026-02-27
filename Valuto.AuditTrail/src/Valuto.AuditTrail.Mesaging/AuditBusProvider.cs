using MassTransit;
using Microsoft.Extensions.Options;
using System;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Mesaging.Interfaces;
using Valuto.AuditTrail.Mesaging.Options;

#if NETSTANDARD2_0 || NET47 || NETCOREAPP3_1 || NET6_0 || NET7_0
using GreenPipes;
#endif

namespace Valuto.AuditTrail.Mesaging
{
    internal class AuditBusProvider : IAuditBusProvider, IDisposable
    {
        private readonly IBusControl _busControl;
        private bool _isStarted;

        public AuditBusProvider(IOptions<AuditOptions> options)
        {
            var opt = options.Value;

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

                cfg.Message<AuditTrailCreatedEvent>(m => m.SetEntityName(options.Value.Exchange));
                cfg.Publish<AuditTrailCreatedEvent>(x =>
                {
                    x.BindQueue(options.Value.Exchange, options.Value.Queue);
                });

                cfg.Message<AuditLogEvent>(m =>
                {
                    m.SetEntityName(options.Value.LogExchange);
                });

                cfg.Publish<AuditLogEvent>(x =>
                {
                    x.BindQueue(options.Value.LogExchange, options.Value.LogQueue);
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
            if (!_isStarted)
            {
                _busControl.Start();
                _isStarted = true;
            }
            return _busControl;
        }

        public void Dispose() => _busControl?.Stop(TimeSpan.FromSeconds(5));
    }
}