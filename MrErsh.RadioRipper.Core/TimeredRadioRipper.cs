using JetBrains.Annotations;
using MrErsh.RadioRipper.Model;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MrErsh.RadioRipper.Core
{
    public class TimeredRadioRipper : IDisposable
    {
        #region Fields

        private readonly IRadioRipper _radioRipper;
        private readonly ILogger _logger;

        private readonly Station _station;
        private System.Timers.Timer _timer;
        private CancellationTokenSource _cts;
        private RipperSettings _settings;
        private int _isReading;
        private string _prevTitle;

        #endregion

        #region Constructor

        public TimeredRadioRipper(IRadioRipper radioRipper, [NotNull] Station station, ILogger logger)
        {
            _radioRipper = radioRipper;
            _station = station;
            _logger = logger;
        }

        #endregion

        public Func<TrackChangedEventArg, Task> TrackChangedAsync { get; set; }

        public Guid StationId => _station.Id;

        #region Implementation of IDispose

        public void Dispose() => Stop();

        #endregion

        #region Methods

        public void Run([NotNull] RipperSettings settings)
        {
            Stop();

            _settings = settings;
            _cts = new CancellationTokenSource();
            _timer = new System.Timers.Timer(settings.Interval * 1000) { AutoReset = true };
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void Stop()
        {
            _prevTitle = null;

            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _isReading = 0;
        }

        #endregion

        #region Event Handlers

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref _isReading, 1, 0) != 0)
                return;

            var token = _cts?.Token ?? CancellationToken.None;

            try
            {
                var headerInfo = await _radioRipper.ReadHeaderAsync(_station.Url, _settings, token).ConfigureAwait(false);
                var title = headerInfo?.StreamTitle;

                if (string.IsNullOrWhiteSpace(title))
                {
                    _logger.Warning("Stream title is null or whitespace. {Origin}. {Url}",
                                    headerInfo?.Origin,
                                    _station.Url);

                    return;
                }
                else if (_prevTitle != title)
                {
                    if (TrackChangedAsync != null)
                        await TrackChangedAsync(new TrackChangedEventArg(_station.Id, headerInfo)).ConfigureAwait(false);

                    _prevTitle = title;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Reading header info error for {Url}", _station.Url);
            }
            finally
            {
                Interlocked.Exchange(ref _isReading, 0);
            }
        }

        #endregion
    }
}
