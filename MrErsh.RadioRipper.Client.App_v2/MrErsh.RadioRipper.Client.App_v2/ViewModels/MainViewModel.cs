using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MrErsh.RadioRipper.Client.Mvvm;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Client.Shared.Messages;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly IThreadingService _threadingService;

        public MainViewModel(INavigationService navigationService,
                             IAuthService authService,
                             IThreadingService threadingService,
                             IMessenger bus)
        {
            _navigationService = navigationService;
            _authService = authService;
            _threadingService = threadingService;

            bus.Register<LogInEvent>(this, (_) => LogEventHandler(true));
            bus.Register<LogOutEvent>(this, (_) => LogEventHandler(false));

            LogOutCommand = new RelayCommand(() => _authService.LogOut());

            authService.TryLoginAsync();
            _threadingService = threadingService;
        }

        public ICommand LogOutCommand { get; }

        public string CurrentUserName => _authService.CurrentUserName;

        public bool IsAuthorized => _authService.IsAuthorized == true;

        private void LogEventHandler(bool isAuthorized)
        {
            _threadingService.OnUiThread(() =>
            {
                RaisePropertyChanged(nameof(IsAuthorized));
                RaisePropertyChanged(nameof(CurrentUserName));
            });
            
            _navigationService.NavigateTo(isAuthorized ? Pages.Stations : Pages.Login);
        }
    }
}
