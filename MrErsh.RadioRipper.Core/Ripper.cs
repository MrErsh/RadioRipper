using JetBrains.Annotations;
using Serilog;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace MrErsh.RadioRipper.Core
{
    public class Ripper : IRadioRipper
    {
        private readonly ILogger _logger;

        public Ripper(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get track title from current Url
        /// </summary>
        [NotNull]
        public MetadataHeader ReadHeader(string url, [NotNull] RipperSettings settings, CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(url);
            for (var counter = 0; counter < settings.NumOfAttempts; counter++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Debug("Cancellation requested for {Url}", url);
                    return null;
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    using (var socketStream = response.GetResponseStream())
                    {
                        var buffer = new byte[512];
                        int metadataLength = 0;
                        StringBuilder metadataHeader = new();
                        int count = 0;
                        var metaintHeader = response.GetResponseHeader("icy-metaint"); // blocksize of mp3 data
                        int metaInt = Convert.ToInt32(metaintHeader);

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            // read byteblock
                            int bufLen = socketStream.Read(buffer, 0, buffer.Length);
                            if (bufLen < 0)
                                throw new Exception("Buf len is negative");

                            for (int i = 0; i < bufLen; i++)
                            {
                                // if there is a header, the 'headerLength' would be set to a value != 0. Then we save the header to a string
                                if (metadataLength != 0)
                                {
                                    metadataHeader.Append(Convert.ToChar(buffer[i]));
                                    var header = metadataHeader.ToString();
                                    metadataLength--;
                                    if (metadataLength == 0) // all metadata informations were written to the 'metadataHeader' string
                                        return new MetadataHeader(header);
                                }
                                else
                                {
                                    if (!(count++ < metaInt)) // write bytes to filestream
                                                              // get headerlength from lengthbyte and multiply by 16 to get correct headerlength
                                    {
                                        metadataLength = Convert.ToInt32(buffer[i]) * 16;
                                        count = 0;
                                    }
                                }
                            }
                        }
                        socketStream.Close();
                    }
                }
                catch(Exception ex)
                {
                    _logger.Warning(ex, "Attempt {Attempt} for {Url}", counter, url);
                    if (counter == 0)
                        throw;
                }
            }

            return null;
        }

        private static WebRequest CreateRequest(string server)
        {
            var request = (HttpWebRequest)WebRequest.Create(server);
            request.Headers.Add("GET", "/HTTP/1.0");
            request.Headers.Add("Icy-MetaData", "1"); // needed to receive metadata informations
            request.UserAgent = "WinampMPEG/5.09";
            return request;
        }
    }
}
