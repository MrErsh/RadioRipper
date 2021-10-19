using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Logging;
using MrErsh.RadioRipper.Client.Model;
using MrErsh.RadioRipper.Client.Mvvm;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using MrErsh.RadioRipper.Shared;
using PropertyChanged;
using Serilog;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class AddStationViewModel : BaseViewModel, IHasLabel
    {
        #region Fields

        private readonly IThreadingService _threadingService;
        private readonly IRadioRipper _ripper;
        private readonly IApiClient _apiClient;
        private readonly ILogger _logger;

        private readonly RipperSettings _riperSettings = new(0, 10);
        private CancellationTokenSource _checkingCancellation;

        #endregion

        #region Constructor

        public AddStationViewModel(IThreadingService threadingService,
                                   IRadioRipper ripper,
                                   IApiClientFactory _apiFactory,
                                   ILogger logger)
        {
            _threadingService = threadingService;
            _ripper = ripper;
            _apiClient = _apiFactory.Create();
            _logger = logger;

            CheckCommand = new AsyncCommand(CheckAsync);
            CancelCommand = EmptyCommand.Create(Localization.CANCEL);
            AddStationCommand = new AsyncCommand(AddStationAsync,
                                                 AddStationCanExecute,
                                                 Station,
                                                 () => Station.Name,
                                                 () => Station.Url);
        }

        #endregion

        #region Implementation of IHasLabel

        public string Label => Localization.ADD_STATION_DIALOG_TITLE;

        #endregion

        #region Properties

        [NotNull]
        public StationObservable Station { get; private set; } = new StationObservable();

        [OnChangedMethod(nameof(OnUrlChanged))]
        public string Url { get; set; }

        public string CurrentTrackName { get; set; }

        // TODO:
        public CheckingState CheckingUrlState
        { 
            get; 
            set;
        } = CheckingState.Undefined;

        public AsyncCommand CheckCommand { get; }

        public AsyncCommand AddStationCommand { get; }

        public ICommand CancelCommand { get; }

        #endregion

        #region Command actions

        private async Task CheckAsync()
        {
            if (Url.IsNullOrWhiteSpace())
                return;

            _checkingCancellation?.Cancel();
            _checkingCancellation = new CancellationTokenSource();
            CheckingUrlState = CheckingState.InProgress;

            var isError = false;
            string title = null;
            try
            {
                var count = 3;
                while (count > 0)
                {
                    var header = await Task.Run(() => _ripper.ReadHeader(Station.Url, _riperSettings))
                                           .ConfigureAwait(false);

                    title = header.StreamTitle;
                    await Task.Delay(100);
                    count--;
                }
            }
            catch
            {
                isError = true;
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _threadingService.OnUiThread(() =>
            {
                if(_checkingCancellation.IsCancellationRequested)
                {
                    CurrentTrackName = null;
                    CheckingUrlState = CheckingState.Undefined;
                    return;
                }

                if (isError /*|| title == null*/) // uncomment
                {
                    CurrentTrackName = null;
                    CheckingUrlState = CheckingState.Incorrect;
                    return;
                }

                CurrentTrackName = title;
                CheckingUrlState = CheckingState.Correct;
                
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task AddStationAsync()
        {
            using (new LoadingTracker(this))
            {
                if (string.IsNullOrWhiteSpace(Station.Name) || string.IsNullOrWhiteSpace(Station.Url))
                    return;

                var dto = new AddStationDto(Station.Station.Name, Station.Station.Url);
                var response = await _apiClient.AddStationAsync(dto).ConfigureAwait(false);
                _logger.Information("{Event}. {Status}. {@Station}",
                                    LoggingEvents.ADD_STATION,
                                    response.StatusCode,
                                    response?.Content);
            }
        }

        public bool AddStationCanExecute()
            => Station.Name.NotNullOrWhiteSpace() && Station.Url.NotNullOrWhiteSpace();

        #endregion

        #region Private methods

        public void OnUrlChanged()
        {
            Station.Url = Url;
            CheckingUrlState = CheckingState.Undefined;
            CheckCommand.RaiseCanExecuteChanged();
        }

        private void Reset()
        {
            _threadingService.OnUiThread(() =>  
            {
               Station = new StationObservable(new Station());
               CurrentTrackName = null;
               Url = null;
               CheckingUrlState = CheckingState.Undefined;
            });
        }

        private void StopChecking()
        {
            _threadingService.OnUiThread(() => CheckingUrlState = CheckingState.Undefined);
            _checkingCancellation?.Cancel();
        }

        #endregion
    }

    public enum CheckingState
    {
        Undefined,
        InProgress,
        Correct,
        Incorrect
    };
}
