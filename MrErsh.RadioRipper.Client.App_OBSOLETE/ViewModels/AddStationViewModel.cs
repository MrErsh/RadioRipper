using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Client.Shared.Mvvm;
using MrErsh.RadioRipper.Core;
using MrErsh.RadioRipper.Model;
using PropertyChanged;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public enum CheckingState
    {
        Undefined,
        InProgress,
        Correct,
        Incorrect
    };

    public sealed class AddStationViewModel : BaseViewModel
    {
        private readonly IThreadingService _threadingService;
        private readonly IRadioRipper _ripper;

        private CancellationTokenSource _checkingCancellation;

        public AddStationViewModel(IThreadingService threadingService, IRadioRipper ripper)
        {
            _threadingService = threadingService;
            _ripper = ripper;

            CheckCommand = new RelayCommand(async () => await CheckAsync().ConfigureAwait(false));
        }

        #region Properties
        [NotNull]
        public Station Station { get; } = new Station()
        {
            Name = $"My Radio",
            Url = $""
        };

        [OnChangedMethod(nameof(OnUrlChanged))]
        public string Url { get; set; }

        public string CurrentTrackName { get; set; }

        // TODO VE:
        public CheckingState CheckingUrlState { 
            get; 
            set; }
            = CheckingState.Undefined;

        public ICommand CheckCommand { get; }

        #endregion

        public void OnUrlChanged()
        {
            Station.Url = Url;
            CheckingUrlState = CheckingState.Undefined;
            CheckAsync();
        }

        // TODO VE: from navigated
        private async Task CheckAsync()
        {
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
                    var header = await Task.Run(() => _ripper.ReadHeader(Station.Url, new RipperSettings(0, 10)))
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
        }

        private void StopChecking()
        {
            CheckingUrlState = CheckingState.Undefined;
            _checkingCancellation?.Cancel();
        }
    }
}
