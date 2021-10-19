using JetBrains.Annotations;
using MrErsh.RadioRipper.Model;
using Serilog;
using System;
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
        private Timer _timer;
        private RipperSettings _settings;
        private bool _isReading;
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

        public event EventHandler<TrackChangedEventArg> TrackChanged;

        public Guid StationId => _station.Id;

        #region Implementation of IDispose

        public void Dispose() => ((IDisposable)_timer).Dispose();

        #endregion

        #region Methods

        public void Run([NotNull] RipperSettings settings)
        {
            Stop();

            _settings = settings;
            _timer = new Timer(settings.Interval * 1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        public void Stop()
        {
            _prevTitle = null;
            _timer?.Stop();
            _timer?.Dispose();
            _isReading = false;
        }

        #endregion

        #region Event Handlers

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isReading)
                return;

            _isReading = true;

            try
            {
                var headerInfo = await Task.Run(() => _radioRipper.ReadHeader(_station.Url, _settings));
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
                    TrackChanged?.Invoke(this, new TrackChangedEventArg(headerInfo));
                    _prevTitle = title;
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Reading header info error for {Url}", _station.Url);
            }
            finally { _isReading = false; }
        }

        #endregion
    }
}
