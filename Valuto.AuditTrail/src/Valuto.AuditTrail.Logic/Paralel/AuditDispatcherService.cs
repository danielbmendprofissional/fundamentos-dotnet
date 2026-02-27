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

namespace Valuto.AuditTrail.Logic.Paralel
{
    public sealed class AuditDispatcherService<T> : IHostedService, IDisposable where T : class
    {
        private readonly IAuditBuffer<T> _buffer;
        private readonly IServiceProvider _serviceProvider;
        private readonly SemaphoreSlim _maxParallelism;
        private readonly int _batchSize;
        private readonly ConcurrentBag<Task> _activeTasks = new ConcurrentBag<Task>();

        private CancellationTokenSource? _cts = null;
        private Task? _executingTask = null;

        public AuditDispatcherService(
            IAuditBuffer<T> buffer,
            IServiceProvider serviceProvider,
            IOptions<BufferOptions> options)
        {
            var config = options.Value;
            _buffer = buffer;
            _serviceProvider = serviceProvider;
            _maxParallelism = new SemaphoreSlim(config.MaxParallelism);
            _batchSize = config.BatchSize;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ExecuteInternalAsync(_cts.Token);

            return Task.CompletedTask;
        }

        private async Task ExecuteInternalAsync(CancellationToken ct)
        {
            try
            {
                await foreach (var message in _buffer.ReadAllAsync(ct))
                {
                    await _maxParallelism.WaitAsync(ct);
                    var batch = await BuildBatchAsync(message);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var bus = scope.ServiceProvider.GetRequiredService<IBusService<T>>();
                                await bus.PublishAsync(batch, CancellationToken.None);
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
                await Task.WhenAll(_executingTask!, Task.WhenAll(_activeTasks)).ConfigureAwait(false);
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
