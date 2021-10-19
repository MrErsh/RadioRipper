using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Client.Shared.Mvvm;
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class TracksViewModel : BaseViewModel
    {
        private readonly IApiClient _apiClient;
        private readonly IThreadingService _threadingService;

        private Station _station;

        public TracksViewModel(IApiClient apiClient, IThreadingService threadingService)
        {
            _apiClient = apiClient;
            _threadingService = threadingService;

            UpdateCommand = new RelayCommand(async () => await UpdateAsync());
        }

        public ICollection<TrackDto> Tracks { get; private set; }

        [DependsOn(nameof(Tracks))]
        public int TrackCount => Tracks?.Count ?? 0;

        public DateTime DateFrom { get; set; } = DateTime.Now.Date.AddDays(-1);

        public DateTime DateTo { get; set; } = DateTime.Now.Date;

        public ICommand UpdateCommand { get; set; }

        public async Task UpdateAsync([CanBeNull] Station station)
        {
            _station = station;
            await UpdateAsync();
        }

        private async Task UpdateAsync()
        {
            IReadOnlyCollection<TrackDto> tracks = new TrackDto[0];
            if (_station != null)              
                tracks = await _apiClient.GetTracksAsync(_station.Id, 
                                                        new DateTimeOffset(DateFrom).ToUniversalTime(),
                                                        new DateTimeOffset(DateTo).ToUniversalTime())
                                         .ConfigureAwait(false);

            _threadingService.OnUiThread(() => Tracks = tracks.ToList());
        }
    }
}
