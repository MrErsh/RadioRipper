using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.WebApi.Bl
{
    public sealed class RipperReconcileHostedService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        private readonly IRipperManager _ripperManager;
        private readonly ILogger<RipperReconcileHostedService> _logger;

        public RipperReconcileHostedService(IRipperManager ripperManager, ILogger<RipperReconcileHostedService> logger)
        {
            _ripperManager = ripperManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _ripperManager.RestartAllAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ripper startup restart failed");
            }

            using var timer = new PeriodicTimer(Interval);
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
                {
                    try
                    {
                        await _ripperManager.ReconcileAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ripper reconcile failed");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
