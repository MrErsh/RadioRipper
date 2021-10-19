using MrErsh.RadioRipper.Client.Services;
using MrErsh.RadioRipper.Model.Dto;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MrErsh.RadioRipper.Client.Api
{
    public class LoginService : ILoginService
    {
        private readonly IAppSettings _appSettings;
        private readonly ILogger _logger;

        public LoginService(IAppSettings appSettings,
                            ILogger logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        /// <exception cref="AuthorizationException" />
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var uri = new Uri(new Uri(_appSettings.ActualApiHost), "/api/Account");
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(loginDto),
                                            Encoding.UTF8,
                                            "application/json");

            var response = await client.PostAsync(uri, content).ConfigureAwait(false);

            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("Login error. StatusCode: {statusCode}. {message}",
                                response.StatusCode,
                                response.ReasonPhrase);

                throw new AuthorizationException(result);
            }

            return result;
        }
    }
}
