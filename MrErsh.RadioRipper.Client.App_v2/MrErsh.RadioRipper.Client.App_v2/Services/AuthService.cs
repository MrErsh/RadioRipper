using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Api.Infrastructure;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Client.Shared.Messages;
using MrErsh.RadioRipper.Model.Dto;
using MrErsh.RadioRipper.Shared;
using PropertyChanged;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Services
{
    [AddINotifyPropertyChangedInterface]
    public class AuthService : IAuthService
    {
        private readonly ILoginService _loginService;
        private readonly ICredentialStorage _credentialStorage;
        private readonly IMessenger _messenger;
        private readonly IJWTTokenStorage _jwtTokenStorage;
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;
        private readonly INavigationService _navigationService;

        public AuthService(ILoginService loginService,
                           ICredentialStorage credentialStorage,
                           IMessenger messenger,
                           IJWTTokenStorage jwtTokenStorage,
                           IMessageService messageService,
                           ILogger logger,
                           INavigationService navigationService)
        {
            _loginService = loginService;
            _credentialStorage = credentialStorage;
            _messenger = messenger;
            _jwtTokenStorage = jwtTokenStorage;
            _messageService = messageService;
            _logger = logger;
            _navigationService = navigationService;
        }

        // TODO: enum
        public bool? IsAuthorized { get; private set; }

        public string CurrentUserName { get; private set; }

        /// <returns>JWT token</returns>
        [ItemCanBeNull]
        public async Task<string> LoginAsync(string userName, string password, bool createNew)
        {
            try
            {
                string token = null;
                var response = await _loginService
                    .LoginAsync(new LoginDto(userName, password, createNew))
                    .ConfigureAwait(false);

                if (response.IsNullOrWhiteSpace())
                    throw new AuthorizationException();

                token = response;
                IsAuthorized = true;
                CurrentUserName = userName;
                _credentialStorage.Store(userName, password);
                _jwtTokenStorage.Set(token);
                _messenger.Send(new LogInEvent(SharedConstants.DEFAULT_HOST));
                return token;
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync(string.Empty, ex.Message);
                _logger.Warning(ex, ex.Message);
                LogOut();
                return null;
            }
        }

        public async Task TryLoginAsync()
        {
            var cred = _credentialStorage.GetLoginInfo();
            if (cred == null)
            {
                LogOut();
                return;
            }
            else
            {
                try
                {
                    await LoginAsync(cred.UserName, cred.Password, false);
                }
                catch (Exception)
                {
                    LogOut();
                }
            }
        }

        public void LogOut()
        {
            if (IsAuthorized == null)
            {
                IsAuthorized = false;
                CurrentUserName = null;
                _navigationService.NavigateTo(Pages.Login);
                return;
            }

            if (IsAuthorized == false)
                return;

            _credentialStorage.Clear();
            _jwtTokenStorage.Reset();
            IsAuthorized = false;
            CurrentUserName = null;
            _messenger.Send(new LogOutEvent());
            return;
        }
    }
}
