using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Mvvm;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Model;
using MrErsh.RadioRipper.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class TracksViewModel : BaseViewModel
    {
        // TODO: from server config
        public DateTimeOffset MinDate { get; set; } = new DateTimeOffset(DateTime.Today.AddMonths(-1).AddDays(-5));
        public DateTimeOffset MaxDate { get; set; } = new DateTimeOffset(DateTime.Today);

        private readonly IApiClient _apiClient;
        private readonly IThreadingService _threadingService;

        private Station _station;

        public TracksViewModel(IApiClientFactory apiFactory, IThreadingService threadingService)
        {
            _apiClient = apiFactory.Create();
            _threadingService = threadingService;

            RefreshCommand = new ExecutionCommand<object>(RefreshAsync);
            CopyTracksCommand = new RelayCommand(
                () => ClipboardHelper.CopyList(Tracks?.Select(t => t.FullName)));

            SelectedDate = DateTime.Today;
        }

        public ICollection<TrackDto> Tracks { get; private set; }
        
        public int TrackCount => Tracks?.Count ?? 0;

        public IReadOnlyCollection<DateTime> Dates { get; set; }

        public DateTimeOffset SelectedDate { get; set; }

        public ExecutionCommand<object> RefreshCommand { get; }

        public ICommand CopyTracksCommand { get; }

        public async Task UpdateAsync([CanBeNull] Station station)
        {
            _station = station;
            await UpdateAvailableDates();
            await RefreshCommand.ExecuteAsync(null);
        }

        public async Task UpdateAvailableDates()
        {
            if (_station == null || _station.Id == default)
                return;

            var dates = await _apiClient.GetDatesAsync(_station.Id)
                                        .ConfigureAwait(false);

            if (!dates.Contains(DateTime.Today))
                dates.Add(DateTime.Today);

            dates = dates.Select(d => d.Date)
                         .OrderByDescending(d => d)
                         .ToList();

            var date = SelectedDate;
            await _threadingService.OnUiThread(() => Dates = dates );
        }

        private async Task<object> RefreshAsync()
        {
            var date = SelectedDate;

            IReadOnlyCollection<TrackDto> tracks = Array.Empty<TrackDto>();
            await _threadingService.OnUiThread(() => Tracks = tracks.ToList());
            if (_station == null)
                return null;

            tracks = await _apiClient.GetTracksAsync(_station.Id, date, date.AddDays(1).AddSeconds(-1))
                                     .ConfigureAwait(false);

            await _threadingService.OnUiThread(() => Tracks = tracks.ToList());

            return null;
        }
    }
}
