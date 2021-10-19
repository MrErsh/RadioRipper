using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using Serilog;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Infrastructure.Http
{
    public class HttpErrorHandler : DelegatingHandler
    {
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public HttpErrorHandler([NotNull] IMessageService messageService,
                                [NotNull] ILogger logger,
                                [NotNull] HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _messageService = messageService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(request, cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                var message = result.StatusCode == System.Net.HttpStatusCode.Forbidden
                    ? "Insufficient privilegies"
                    : result.ReasonPhrase;
                await _messageService.ShowErrorAsync("Error", message).ConfigureAwait(false);
                _logger.Error("HttpErrorHandler: ", message);
            }

            return result;
        }
    }
}
