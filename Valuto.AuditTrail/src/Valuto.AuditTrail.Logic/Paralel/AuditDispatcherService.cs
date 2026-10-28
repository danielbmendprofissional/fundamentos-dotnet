using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Configuration;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Mesaging.Interfaces;

namespace Valuto.AuditTrail.Logic.Paralel
{
    internal sealed class AuditDispatcherService<T> : IHostedService, IDisposable where T : class
    {
        private readonly IAuditBusProvider _busProvider;
        private readonly IAuditBuffer<T> _buffer;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _maxParallelism;
        private readonly int _batchSize;
        private readonly ConcurrentBag<Task> _activeTasks = new ConcurrentBag<Task>();

        private CancellationTokenSource? _cts = null;
        private Task? _executingTask = null;

        public AuditDispatcherService(
            IAuditBusProvider busProvider,
            IAuditBuffer<T> buffer,
            IServiceProvider serviceProvider,
            IOptions<BufferOptions> options)
        {
            var config = options.Value;
            _buffer = buffer;
            _serviceProvider = serviceProvider;
            _maxParallelism = new SemaphoreSlim(config.MaxParallelism);
            _batchSize = config.BatchSize;
            _busProvider = busProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var bus = _busProvider.GetBus();
            await bus.StartAsync(cancellationToken).ConfigureAwait(false);

            _executingTask = ExecuteInternalAsync(_cts.Token);

            await Task.CompletedTask;
        }

        private async Task ExecuteInternalAsync(CancellationToken ct)
        {
            try
            {
                await foreach (var message in _buffer.ReadAllAsync(ct).ConfigureAwait(false))
                {
                    await _maxParallelism.WaitAsync(ct).ConfigureAwait(false);
                    var batch = await BuildBatchAsync(message);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var bus = scope.ServiceProvider.GetRequiredService<IBusService<T>>();
                                await bus.PublishAsync(batch, CancellationToken.None).ConfigureAwait(false);
                            }
                        }
                        finally { _maxParallelism.Release(); }
                    }, CancellationToken.None);

                    _activeTasks.Add(task);
                    CleanCompletedTasks();
                }
            }
            catch { }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _buffer.Complete();
            _cts?.Cancel();

            try
            {
                if (_executingTask != null)
                {
                    await Task.WhenAll(_executingTask!, Task.WhenAll(_activeTasks)).ConfigureAwait(false);
                }

                await _busProvider.GetBus().StopAsync(cancellationToken).ConfigureAwait(false);
            }
            catch { }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _maxParallelism?.Dispose();
        }

        private Task<List<T>> BuildBatchAsync(T firstItem)
        {
            var batch = new List<T> { firstItem };
            while (batch.Count < _batchSize && _buffer.TryRead(out var extraItem))
            {
                batch.Add(extraItem);
            }

            return Task.FromResult(batch);
        }

        private void CleanCompletedTasks()
        {
            while (_activeTasks.TryPeek(out var t) && t.IsCompleted)
            {
                _activeTasks.TryTake(out _);
            }
        }
    }
}
