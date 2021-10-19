using JetBrains.Annotations;
using MrErsh.RadioRipper.Client.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Infrastructure.Http
{
    public class ErrorHandler : DelegatingHandler
    {
        private readonly IMessageService _messageService;

        public ErrorHandler([NotNull] IMessageService messageService, [NotNull] HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _messageService = messageService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                return response;
            }
            catch (Exception ex)
            {
                await _messageService.ShowErrorAsync("Error", $"Unknown error. {ex.Message}").ConfigureAwait(false);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
