using GalaSoft.MvvmLight.Messaging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Api.Infrastructure;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Client.ViewModels;
using MrErsh.RadioRipper.Core;
using Serilog;
using System;
using System.Threading;

#if DEBUG_FAKEDATA
using MrErsh.RadioRipper.Client.Api.Fake;
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MrErsh.RadioRipper.Client.App_v2
{
    public partial class App : Application
    {
        private static readonly SemaphoreSlim _dispatcherSemaphore = new(0, 1);
        private static readonly SemaphoreSlim _navigationSemaphore = new(0, 1);

        private MainWindow m_window;

        public App()
        {
            InitializeComponent();
            UnhandledException += (s, e) =>
            {
                Log.Logger.Fatal(e.Exception, "Fatal error");
            };
            InitializeContainer(GetDispatcherQueue, GetXamlRoot, GetContentFrame);
        }

        private DispatcherQueue GetDispatcherQueue() => m_window.DispatcherQueue;
        private XamlRoot GetXamlRoot() => m_window.Content.XamlRoot;
        private Frame GetContentFrame() => m_window.RootFrame;

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Closed += (s, e) => Log.CloseAndFlush();
            m_window.Activate();
            m_window.Title = "RadRIP";
            _navigationSemaphore.Release();
            _dispatcherSemaphore.Release();
            m_window.RootFrame.IsNavigationStackEnabled = false;
        }

        private static void InitializeContainer(Func<DispatcherQueue> dispFactory, Func<XamlRoot> xamlRootFactory, Func<Frame> contentFrameFactory)
        {

#if DEBUG
            var logPath = @"C:\logs\RadioRipper\App\log-.txt";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.Async(c => c.File(logPath, rollingInterval: RollingInterval.Day))
                .CreateLogger();
#else
           var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           var logPath = Path.Combine(asmDir, "logs");
           // use Serilog.Formatting.Compact may be
           // Serilog.Sinks.ApplicationInsights
           Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
#endif
            var logger = Log.Logger;
            var builder = new ServiceContainer.ContainerConfigurator();
            var threadingService = new OnDispatcherQueueThreadingService(dispFactory, _dispatcherSemaphore);
            var dlgService = new DialogService(threadingService, logger, xamlRootFactory);
            var credentialStorage = new CredentialStorage(logger);
            builder.RegisterSingleton<IThreadingService>(() => threadingService)
                   .RegisterSingleton<IMessageService>(() => dlgService)
                   .RegisterSingleton<IDialogService>(() => dlgService)
                   .Register<IRadioRipper, Ripper>()
                   .RegisterSingleton<IAuthService, AuthService>()
                   .RegisterSingleton<ICredentialStorage>( () => credentialStorage)
                   .RegisterSingleton<IMessenger>(() => Messenger.Default)
                   .RegisterSingleton<INavigationService>(() => new SimpleNavigationService(threadingService, logger, contentFrameFactory, _navigationSemaphore))
                   .RegisterSingleton<ILogger>(() => Log.Logger)
                   .RegisterSingleton<IAppSettings, AppSettings>()
                   .RegisterSingleton<IJWTTokenStorage, JWTTokenStorage>()
                   ;
#if DEBUG_FAKEDATA
            builder.RegisterSingleton<IApiClientFactory, FakeApiClientFactory>()
                   .RegisterSingleton<ILoginService, FakeLoginService>();
#else
            builder.RegisterSingleton<IApiClientFactory, ApiClientFactory>()
                   .RegisterSingleton<ILoginService, LoginService>();
#endif
            RegisterViewModels(builder);
            builder.Configure();
        }

        private static void RegisterViewModels(ServiceContainer.ContainerConfigurator configurator)
        {
            configurator.RegisterSingleton<MainViewModel>()
                        .Register<StationsViewModel>()
                        .Register<TracksViewModel>()
                        .Register<LoginViewModel>()
                        .Register<AddStationViewModel>();
        }
    }
}
