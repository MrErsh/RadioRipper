using JetBrains.Annotations;
using Serilog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Infrastructure.Http
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public LoggingHandler([NotNull] ILogger logger, [NotNull] HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            try
            {
                var method = request.Method.Method;
                _logger.Debug(">> {Guid}: {Method}: {RequestUri}", guid, method, request.RequestUri);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                _logger.Debug("<< {Guid}: {Method}: {StatusCode} {Content}", guid, method, (int)response.StatusCode, response.Content);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "!! {Guid}", guid);
                throw;
            }
        }
    }
}
