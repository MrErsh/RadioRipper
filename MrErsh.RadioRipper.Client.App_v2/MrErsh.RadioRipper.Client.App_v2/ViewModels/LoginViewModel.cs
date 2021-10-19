using MrErsh.RadioRipper.Client.Mvvm;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Client.Shared;
using MrErsh.RadioRipper.Shared;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MrErsh.RadioRipper.Client.ViewModels
{
    public class LoginViewModel : BaseViewModel, INavigable
    {
        #region Fields

        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IThreadingService _threadingService;
        private readonly IAppSettings _appSettings;
        private readonly IMessageService _messageService;

        #endregion

        #region Constructor
        public LoginViewModel(IAuthService authService,
                              ILogger logger,
                              IThreadingService threadingService,
                              IAppSettings appSettings,
                              IMessageService messageService)
        {
            _authService = authService;
            _logger = logger;
            _threadingService = threadingService;
            _appSettings = appSettings;
            _messageService = messageService;

            ResetToDefaultCommand = new AsyncCommand(ResetToDefaultAction);
            LoginCommand = new ExecutionCommand<bool>(
                LoginAction,
                LoginCanExecute,
                this,
                () => Login,
                () => Password,
                () => ApiHost)
            {
                Label = Localization.LOGIN
            };

//#if DEBUG || DEBUG_FAKEDATA
//            Login = "admin";
//            Password = "Admin123!@#";
//#endif
        }

        #endregion

        #region Properties

        public string Login { get; set; }

        public string Password { get; set; }

        public bool CreateNew { get; set; }

        public string ApiHost { get; set; }

        public ExecutionCommand<bool> LoginCommand { get; }

        public ICommand ResetToDefaultCommand { get; }

        #endregion

        #region Implementation of INavigable

        public override Task NavigatedTo()
        {
            ApiHost = _appSettings.ActualApiHost;
            return base.NavigatedTo();
        }

        #endregion

        #region Private methods

        private async Task<bool> LoginAction()
        {
            try
            {
                _appSettings.Host = ApiHost.IsNullOrWhiteSpace() ? null : ApiHost;
                await _authService.LoginAsync(Login, Password, CreateNew).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Login error");
                throw;
            }

            return true;
        }

        private bool LoginCanExecute()
            => Login.NotNullOrWhiteSpace()
               && Password.NotNullOrWhiteSpace()
               && ApiHost.NotNullOrWhiteSpace();

        private async Task ResetToDefaultAction()
        {
            var isOk = await _messageService.ShowMessageAsync(string.Empty,
                                                              Localization.RESET_HOST_TO_DEFAULT_CONFIRMATION);
            if (!isOk)
                return;

            await _threadingService.OnUiThread(() => ApiHost = SharedConstants.DEFAULT_HOST);
        }

        #endregion
    }
}
