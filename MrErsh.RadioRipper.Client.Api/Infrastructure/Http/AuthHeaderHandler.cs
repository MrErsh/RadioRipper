using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Api;
using MrErsh.RadioRipper.Client.Api.Infrastructure;
using MrErsh.RadioRipper.Client.Services;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Infrastructure.Http
{
    public sealed class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IJWTTokenStorage _jwtTokenProvider;
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AuthHeaderHandler([NotNull] IJWTTokenStorage jwtTokenProvider,
                                 [NotNull] IAuthService authService,
                                 [NotNull] ILogger logger,
                                 [NotNull] HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _jwtTokenProvider = jwtTokenProvider;
            _authService = authService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            try
            {
                var token = await _jwtTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "AuthHeaderHandler");
                _authService.LogOut();
            }

            return response is null || response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? throw new AuthorizationException()
                : response;
        }
    }
}

