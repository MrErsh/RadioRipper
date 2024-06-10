using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Dialogs;
using MrErsh.RadioRipper.Client.Model;
using MrErsh.RadioRipper.Client.Mvvm;
using MrErsh.RadioRipper.Client.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public sealed class StationsViewModel : BaseViewModel
    {
        #region Fields

        private readonly IApiClient _apiClient;
        private readonly IThreadingService _threadingService;
        private readonly IDialogService _dialogService;
        private readonly IMessageService _messageService;

        #endregion

        #region Constructor
        
        public StationsViewModel(IApiClientFactory apiClientFactory,
                                 IThreadingService threadingService,
                                 TracksViewModel tracksViewModel,
                                 IDialogService dialogService,
                                 IMessageService messageService)
        {
            _apiClient = apiClientFactory.Create();
            _threadingService = threadingService;
            _dialogService = dialogService;
            _messageService = messageService;

            TracksViewModel = tracksViewModel;

            AddStationCommand = new AsyncCommand(AddStationAsync);
            DeleteStationCommand = new ExecutionCommand<Guid?, bool>(DeleteStationAsync);
            ChangeStationStateCommand = new ExecutionCommand<Guid?, bool>(id => ChangeStationState(id));
            RefreshCommand = new ExecutionCommand<bool>(RefreshStationsAsync);
        }

        #endregion

        #region Implementation of INavigable

        public override async Task NavigatedTo()
        {
            await RefreshStationsAsync();
            await base.NavigatedTo();
        }

        #endregion

        #region Properties

        public ICollection<StationModel> Stations { get; set; }

        public bool IsOpened { get; set; }

        [OnChangedMethod(nameof(SelectedStationChanged))]
        public StationModel SelectedStation { get; set; }

        public TracksViewModel TracksViewModel { get; }

        public AddStationViewModel AddStationViewModel { get; }

        public ICommand AddStationCommand { get; }

        public ICommand DeleteStationCommand { get; }

        public AsyncExecutionCommandBase<bool> ChangeStationStateCommand { get; }

        public AsyncExecutionCommandBase<bool> RefreshCommand { get; }

        #endregion

        #region Command actions

        private async Task<bool> DeleteStationAsync(Guid? id)
        {
            if (id.Value == default)
                return true;

            using (new LoadingTracker(this))
            {
                var ok = await _messageService.ShowMessageAsync(string.Empty,
                                                                Localization.DELETE_STATION_CONFIRMATION);
                if (!ok)
                    return true;

                var result = await _apiClient.DeleteStationAsync(id.Value).ConfigureAwait(false);
                if (result.IsSuccessStatusCode)
                    await RefreshStationsAsync();
            }

            return true;
        }

        private async Task AddStationAsync()
        {
            var (isOk, _) = await _dialogService
                .ShowDialogAsync<AddStationViewModel>(
                    DialogTypes.AddStation,
                    vm => vm.AddStationCommand,
                    vm => vm.CancelCommand)
                .ConfigureAwait(false);

            if (isOk)
                await RefreshStationsAsync();
        }

        private async Task<bool> RefreshStationsAsync()
        {
            using (new LoadingTracker(this))
            {
                var curStationId = SelectedStation?.Station.Id;

                var stations = await _apiClient.GetStationsAsync().ConfigureAwait(false);
                var st = stations
                        .Content?
                        .Select(st =>
                            new StationModel
                            {
                                Station = st,
                                IsChecked = st.IsRunning
                            })
                        .OrderByDescending(st => st.Station.IsRunning)
                        .ThenByDescending(st => st.Station?.Name)
                        .ToList();

                await _threadingService.OnUiThreadAsync(() =>
                {
                    Stations = st;
                    return true;
                });
            }

            return true;
        }

        #endregion

        #region Private methods

        private async Task<bool> ChangeStationState(Guid? id)
        {
            try
            {
                if (id == null || id.Value == default)
                    return false;

                var station = Stations?.First(st => st.Station?.Id == id);
                using (new LoadingTracker(this))
                {
                    var newState = !station.Station.IsRunning;
                    var isSuccess = await _apiClient
                        .ChangeIsRunningAsync(id.Value, new ChangeIsRunningParams { IsRunning = newState })
                        .ConfigureAwait(false);

                    if (isSuccess)
                    {
                        station.Station.IsRunning = newState;
                        await _threadingService.OnUiThreadAsync(() =>
                        {
                            station.IsChecked = newState;
                            return true;
                        });
                    }

                    return station.IsChecked;
                }
            }
            catch(Exception ex)
            {
                await RefreshStationsAsync();
                Debug.WriteLine(ex);
                return false;
            }
        }

        private async void SelectedStationChanged()
        {
            await TracksViewModel?.UpdateAsync(SelectedStation?.Station);
            var id = SelectedStation?.Station.Id;
            Console.WriteLine($"SelectedStation: {id}");
        }

        #endregion
    }
}
