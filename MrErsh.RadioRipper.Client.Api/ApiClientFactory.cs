using GalaSoft.MvvmLight.Messaging;
using MrErsh.RadioRipper.Client.Api.Infrastructure;
using MrErsh.RadioRipper.Client.Infrastructure.Http;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Client.Shared.Messages;
using Refit;
using Serilog;
using System;
using System.Net.Http;

namespace MrErsh.RadioRipper.Client.Api
{
    public class ApiClientFactory : IApiClientFactory
    {
        private readonly IJWTTokenStorage _jwtTokenStorage;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IAppSettings _appSettings;
        private readonly IMessageService _messageService;

        private readonly object _monitor = new();

        public ApiClientFactory(IJWTTokenStorage jwtTokenStorage,
                                IAuthService authService,
                                ILogger logger,
                                IMessenger messenger,
                                IAppSettings appSettings,
                                IMessageService messageService)
        {
            _jwtTokenStorage = jwtTokenStorage;
            _authService = authService;
            _logger = logger;
            _appSettings = appSettings;
            _messageService = messageService;

            messenger.Register<LogInEvent>(this, LoginHandler);
            messenger.Register<LogOutEvent>(this, LogOutHandler);
        }

        public IApiClient Create()
        {
            // NewtonsoftJsonContentSerializer т.к. дефолтный из System.Text.Json не может нормально обработать пустую строку (402 - NoContent);
            var settings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            var handler = (HttpMessageHandler)
                new ErrorHandler(_messageService,
                new HttpErrorHandler(_messageService, _logger,
                new AuthHeaderHandler(_jwtTokenStorage, _authService, _logger,
                new LoggingHandler(_logger,
                new HttpClientHandler()
                ))));

            lock (_monitor)
            {
                var baseUri = new Uri(_appSettings.ActualApiHost);
                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(baseUri, "/api"),
                    Timeout = new TimeSpan(0, 0, 10)
                };

                var result = RestService.For<IApiClient>(client, settings);
                return result;
            }
        }

        private void LoginHandler(LogInEvent ev)
        {
            lock (_monitor)
            {
                _appSettings.Host = ev.Host;
            }
        }

        private void LogOutHandler(LogOutEvent _)
        {
            lock (_monitor)
            {
                _appSettings.Host = null;
            }
        }
    }
}
