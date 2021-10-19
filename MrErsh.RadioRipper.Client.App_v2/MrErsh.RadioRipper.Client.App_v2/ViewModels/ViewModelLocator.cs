using MrErsh.RadioRipper.Client.Services;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class ViewModelLocator
    {
#pragma warning disable CA1822 // Mark members as static

        public MainViewModel MainViewModel => ServiceContainer.Get<MainViewModel>();

        public StationsViewModel StationsViewModel => ServiceContainer.Get<StationsViewModel>();

        public TracksViewModel TracksViewModel => ServiceContainer.Get<TracksViewModel>();

        public LoginViewModel LoginViewModel => ServiceContainer.Get<LoginViewModel>();

        public AddStationViewModel AddStationViewModel => ServiceContainer.Get<AddStationViewModel>();

#pragma warning restore CA1822 // Mark members as static
    }
}
