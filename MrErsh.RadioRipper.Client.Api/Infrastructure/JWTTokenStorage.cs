using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Model.Dto;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api.Infrastructure
{
    public sealed class JWTTokenStorage : IJWTTokenStorage, IDisposable
    {
        #region Fields

        private readonly ILoginService _loginService;
        private readonly ICredentialStorage _credentialStorage;

        private string _currentToken;
        private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

        #endregion

        #region Constructor

        public JWTTokenStorage([NotNull] ILoginService loginService,
                               [NotNull] ICredentialStorage credentialStorage)
        {
            _loginService = loginService;
            _credentialStorage = credentialStorage;
        }

        #endregion

        #region Implementation of IJWTTokenStorage

        [ItemCanBeNull]
        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_currentToken != null)
                return _currentToken;

            await _tokenSemaphore.WaitAsync();

            if (_currentToken != null)
                return _currentToken;

            try
            {
                var cred = _credentialStorage.GetLoginInfo();
                if (cred is null)
                    throw new AuthorizationException();

                var loginInfo = new LoginDto(cred.UserName, cred.Password); ;
                var response = await _loginService.LoginAsync(loginInfo).ConfigureAwait(false);
                _currentToken = string.IsNullOrWhiteSpace(_currentToken) ? null : _currentToken;

                return string.IsNullOrEmpty(_currentToken)
                    ? throw new AuthorizationException()
                    : _currentToken;
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        public void Reset() => _currentToken = null;

        public void Set([NotNull] string jwtToken)
        {
            _tokenSemaphore.Wait();
            try
            {
                _currentToken = jwtToken;
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose() => _tokenSemaphore.Dispose();

        #endregion
    }
}
