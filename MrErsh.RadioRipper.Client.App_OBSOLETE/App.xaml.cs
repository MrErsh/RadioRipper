using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Infrastructure;
using MrErsh.RadioRipper.Client.Infrastructure.Http;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Client.ViewModels;
using MrErsh.RadioRipper.Core;
using Refit;
using System;
using System.Net.Http;
using System.Windows;
using ContainerConfigurator = MrErsh.RadioRipper.Client.Infrastructure.ServiceContainer.ContainerConfigurator;

namespace MrErsh.RadioRipper.Client
{
    public partial class App : Application
    {
        public App() : base()
        {
            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var builder = new ContainerConfigurator();
            var threadingService = new ThreadingService(Dispatcher); // !! thread
            var msgService = new MessageService();
            builder.RegisterSingleton<IThreadingService>(() => threadingService);
            builder.RegisterSingleton<IMessageService>(() => msgService);
            InitializeViewModels(builder);

#if DEBUG_FAKEDATA
            builder.RegisterSingleton<IApiClient, Api.Fake.FakeApiClient>();
            //builder.RegisterSingleton<Api.Fake.ApiClientFake, IApiClient>();
#else
            InitializeApiClient(builder, threadingService, msgService);
#endif

            builder.Register<IRadioRipper, Ripper>()
                   .Configure();
        }

        private static void InitializeViewModels(ContainerConfigurator configurator) =>
            configurator.Register<StationsViewModel>()
                        .Register<TracksViewModel>()
                        .Register<LoginViewModel>();

        private static void InitializeApiClient(ContainerConfigurator configurator, IThreadingService threadingService, IMessageService messageService)
        {
            var apiHost = @"https://localhost:5001/api";
            // NewtonsoftJsonContentSerializer т.к. дефолтный из System.Text.Json не может нормально обработать пустую строку (402 - NoContent);
            var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            //var apiClient = RestService.For<IApiClient>(apiHost, settings);
            //configurator.RegisterSingleton(() => apiClient);

            var loginRequests = RestService.For<ILoginRequests>(apiHost, settings);
            var tokenProvider = new JWTAuthTokenProvider(loginRequests, new DialogLoginInfoProvider(threadingService));

            var handler =
                new ErrorHandler(
                    messageService,
                    new HttpErrorHandler( // TOOD VE: при тестах закомментить это
                        messageService,
                        new AuthHeaderHandler(tokenProvider, new HttpClientHandler())
                    )
                );

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(apiHost) };
            var apiClientAuthHandled = RestService.For<IApiClient>(httpClient, settings);
            configurator.RegisterSingleton<IApiClient>(() => apiClientAuthHandled);
        }
    }
}
