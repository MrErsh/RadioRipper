#pragma warning disable CA1416
using JetBrains.Annotations;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    public class OnDispatcherQueueThreadingService : IThreadingService
    {
        private readonly SemaphoreSlim _loadedEvent;
        private readonly Lazy<DispatcherQueue> _dispatcherLazy;
        private DispatcherQueue Dispatcher => _dispatcherLazy.Value;

        public OnDispatcherQueueThreadingService([NotNull] Func<DispatcherQueue> dispatcherFactory,
                                                 [NotNull] SemaphoreSlim loadedEvent)
        {
            _dispatcherLazy = new Lazy<DispatcherQueue>(dispatcherFactory);
            _loadedEvent = loadedEvent;
        }

        public async Task OnUiThread(Action action)
        {
            if (!_dispatcherLazy.IsValueCreated)
            {
                await _loadedEvent.WaitAsync();
                _loadedEvent.Release();
            }

            if (action == null)
                return;

            if (Dispatcher.HasThreadAccess)
                action();
            else
                Dispatcher.TryEnqueue(DispatcherQueuePriority.High, () => action());
        }

        public Task<TResult> OnUiThreadAsync<TResult>(Func<TResult> func)
        {
            var taskCompletionSource = new TaskCompletionSource<TResult>();
            if (func != null)
            {
                if (!_dispatcherLazy.IsValueCreated)
                    _loadedEvent.Wait();

                Dispatcher.TryEnqueue(() =>
                {
                    var result = func();
                    taskCompletionSource.SetResult(result);
                });
            }
            else
            {
                taskCompletionSource.SetResult(default);
            }

            return taskCompletionSource.Task;
        }
    }
}
#pragma warning restore CA1416
