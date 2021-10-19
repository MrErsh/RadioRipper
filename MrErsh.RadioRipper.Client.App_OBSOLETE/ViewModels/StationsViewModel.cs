using GalaSoft.MvvmLight.Command;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Model;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Client.Shared.Mvvm;
using MrErsh.RadioRipper.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public sealed class StationsViewModel : BaseViewModel
    {
        private readonly IApiClient _apiClient;
        private readonly IThreadingService _threadingService;
        private readonly IRadioRipper _ripper;

        public StationsViewModel(IApiClient apiClient, IThreadingService threadingService, IRadioRipper ripper, TracksViewModel tracksViewModel)
        {
            _apiClient = apiClient;
            _threadingService = threadingService;
            _ripper = ripper;
            TracksViewModel = tracksViewModel;
          
            AddStationCommand = new RelayCommand(async () => await AddStationAsync().ConfigureAwait(false));
            DeleteStationCommand = new RelayCommand<Guid>(async (id) => await DeleteStationAsync(id).ConfigureAwait(false));
            RunStationCommand = new RelayCommand<Guid>(async (id) => await RunStationAsync(id).ConfigureAwait(false));
            StopStationCommand = new RelayCommand<Guid>(async (id) => await StopStationAsync(id).ConfigureAwait(false));
            RefreshCommand = new RelayCommand(async () => await RefreshStationsAsync().ConfigureAwait(false));

            Task.Run(() => RefreshStationsAsync());
        }

        public ICollection<StationModel> Stations { get; set; }

        [OnChangedMethod(nameof(SelectedStationChanged))]
        public StationModel SelectedStation { get; set; }

        public TracksViewModel TracksViewModel { get; }

        public ICommand AddStationCommand { get; }

        public ICommand DeleteStationCommand { get; }

        public ICommand RunStationCommand { get; }

        public ICommand StopStationCommand { get; }

        public ICommand RefreshCommand { get; }

        private async Task DeleteStationAsync(Guid? id)
        {
            if (id.Value == default)
                return;

            using (new LoadingTracker(this))
            {
                MessageBoxResult mbResult = MessageBox.Show("Are you shure", "Delete station", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mbResult == MessageBoxResult.No)
                    return;

                var result = await _apiClient.DeleteStationAsync(id.Value);

                if (result.IsSuccessStatusCode)
                    await RefreshStationsAsync();
            }
        }

        private async Task AddStationAsync()
        {
            // TODO VE: use dialog service and injection
            var addDialog = new AddStationWindow();
            var vm = new AddStationViewModel(_threadingService, _ripper);
            addDialog.DataContext = vm;
            var result = addDialog.ShowDialog();
            if (result != true)
                return;

            using (new LoadingTracker(this))
            {
                if (string.IsNullOrWhiteSpace(vm.Station.Name) || string.IsNullOrWhiteSpace(vm.Station.Url))
                    return;

                await _apiClient.AddStationAsync(vm.Station).ConfigureAwait(false);
                await RefreshStationsAsync();
            }
        }

        private async Task RunStationAsync(Guid? id)
        {
            if (id.GetValueOrDefault() == default)
                return;
            
            using (new LoadingTracker(this))
            {
                var result = await _apiClient
                        .ChangeIsRunningAsync(id.Value, new ChangeIsRunningParams { IsRunning = true })
                        .ConfigureAwait(false);

                await RefreshStationsAsync();
            }
        }

        private async Task StopStationAsync(Guid? id)
        {
            if (id.GetValueOrDefault() == default)
                return;

            using (new LoadingTracker(this))
            {
                var result = await _apiClient
                    .ChangeIsRunningAsync(id.Value, new ChangeIsRunningParams { IsRunning = false })
                    .ConfigureAwait(false);

                await RefreshStationsAsync();
            }
        }

        private async Task RefreshStationsAsync()
        {
            using (new LoadingTracker(this))
            {
                var stations = await _apiClient.GetStationsShortAsync().ConfigureAwait(false);
                await Task.Delay(100);
                _threadingService.OnUiThread(() =>
                {
                    Stations = stations
                        .Select(st => new StationModel 
                        { 
                            // TODO VE: fix
                            Station = new RadioRipper.Model.Station {
                                Id = st.Id,
                                Name = st.Name,
                                Url = st.Name,
                                Comment = st.Comment,
                                IsRunning = st.IsRunning
                            }
                        })
                        .OrderByDescending(st => st.Station.IsRunning)
                        .ToList();
                });
            }
        }

        private async void SelectedStationChanged()
        {
            await TracksViewModel?.UpdateAsync(SelectedStation?.Station);
            var id = SelectedStation?.Station.Id;
            Console.WriteLine($"SelectedStation: {id}");
        }
    }
}
