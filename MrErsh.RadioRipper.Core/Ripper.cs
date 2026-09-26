using JetBrains.Annotations;
using Serilog;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Core
{
    public sealed class Ripper : IRadioRipper, IDisposable
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public Ripper(ILogger logger)
            : this(logger, CreateClient())
        {
        }

        private Ripper(ILogger logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        private static HttpClient CreateClient()
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                UseCookies = false,
                AllowAutoRedirect = false,
            };

            return new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
        }

        public async Task<MetadataHeader> ReadHeaderAsync(string url, [NotNull] RipperSettings settings,
                                                          CancellationToken cancellationToken = default)
        {
            var attempts = Math.Max(1, settings.NumOfAttempts);

            for (var attempt = 0; attempt < attempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await ReadOnce(url, settings, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger?.Warning(ex, "Attempt {Attempt}/{Total} failed for {Url}", attempt + 1, attempts, url);

                    if (attempt == attempts - 1)
                        throw;
                }
            }

            return null;
        }

        private async Task<MetadataHeader> ReadOnce(string url, RipperSettings settings, CancellationToken outerCt)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.TryAddWithoutValidation("Icy-MetaData", "1");
            request.Headers.UserAgent.ParseAdd("WinampMPEG/5.09");

            using (var connectCts = CancellationTokenSource.CreateLinkedTokenSource(outerCt))
            {
                connectCts.CancelAfter(settings.ConnectTimeoutMs);

                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, connectCts.Token)
                                            .ConfigureAwait(false);
                using (response)
                {
                    response.EnsureSuccessStatusCode();

                    if (!TryGetIntHeader(response, "icy-metaint", out var metaInt) || metaInt <= 0)
                        return null;

                    using var readCts = CancellationTokenSource.CreateLinkedTokenSource(outerCt);
                    readCts.CancelAfter(settings.ReadTimeoutMs);

                    var stream = await response.Content.ReadAsStreamAsync(readCts.Token).ConfigureAwait(false);
                    return await ReadMetadataBlockAsync(stream, metaInt, readCts.Token).ConfigureAwait(false);
                }
            }
        }

        private static async Task<MetadataHeader> ReadMetadataBlockAsync(Stream stream, int metaInt,
                                                                         CancellationToken ct)
        {
            var buffer = new byte[512];
            var metadata = new StringBuilder();
            var count = 0;
            var metadataLength = 0;

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var bufLen = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false);
                if (bufLen <= 0)
                    throw new IOException("Stream closed before a metadata block was read.");

                for (var i = 0; i < bufLen; i++)
                {
                    if (metadataLength != 0)
                    {
                        metadata.Append(Convert.ToChar(buffer[i]));
                        if (--metadataLength == 0)
                            return new MetadataHeader(metadata.ToString());
                    }
                    else if (!(count++ < metaInt))
                    {
                        metadataLength = buffer[i] * 16;
                        count = 0;
                    }
                }
            }
        }

        private static bool TryGetIntHeader(HttpResponseMessage response, string name, out int value)
        {
            value = 0;
            if (response.Headers.TryGetValues(name, out var values))
            {
                foreach (var v in values)
                {
                    if (int.TryParse(v, out value))
                        return true;
                }
            }

            return false;
        }

        public void Dispose() => _client.Dispose();
    }
}
