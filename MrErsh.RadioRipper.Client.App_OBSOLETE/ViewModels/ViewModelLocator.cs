using MrErsh.RadioRipper.Client.Infrastructure;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    // TODO VE: as MarkupExtension
    public class ViewModelLocator
    {
        public StationsViewModel Stations { get; } = ServiceContainer.Get<StationsViewModel>();

        public TracksViewModel Tracks { get; } = ServiceContainer.Get<TracksViewModel>();

        public LoginViewModel LoginViewModel => ServiceContainer.Get<LoginViewModel>();
    }
}
