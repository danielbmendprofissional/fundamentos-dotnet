using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Configuration;

namespace Valuto.AuditTrail.Logic.Paralel
{
    internal sealed class AuditBuffer<T> : IAuditBuffer<T> where T : class
    {
        private readonly BlockingCollection<T> _queue;

        public AuditBuffer(IOptions<BufferOptions> options)
        {
            var config = options.Value;
            _queue = new BlockingCollection<T>(config.MaxBufferSize);
        }

        public async ValueTask EnqueueAsync(T item, CancellationToken ct = default)
        {
            await Task.Run(() => _queue.Add(item, ct), ct);
        }

        public void Complete() => _queue.CompleteAdding();

        public bool TryRead([System.Diagnostics.CodeAnalysis.System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T item)
        {
            return _queue.TryTake(out item!);
        }

        public async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken ct)
        {
            while (!_queue.IsCompleted)
            {
                if (ct.IsCancellationRequested) break;

                if (_queue.TryTake(out var item))
                {
                    yield return item;
                }
                else
                {
                    try
                    {
                        await Task.Delay(100, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }
    }
}
