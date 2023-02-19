using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MCMS.Common.Turnstile
{
    public class TurnstileValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TurnstileValidationService> _logger;
        private readonly TurnstileConfig _config;

        public TurnstileValidationService(
            IHttpClientFactory httpClientFactory,
            ILogger<TurnstileValidationService> logger,
            IOptions<TurnstileConfig> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = options.Value;
        }

        public async Task<bool> IsValid(string turnstileResponse)
        {
            _logger.LogInformation("Validating a turnstile response...");
            var httpClient = _httpClientFactory.CreateClient();
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", _config.SecretKey),
                new KeyValuePair<string, string>("response", turnstileResponse)
            });
            var httpResponse =
                await httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", formContent);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Verification failed: {Response}", await httpResponse.Content.ReadAsStringAsync());
                return false;
            }

            var response =
                JsonConvert.DeserializeObject<TurnstileResponseModel>(await httpResponse.Content.ReadAsStringAsync());

            if (!response.Success)
            {
                _logger.LogWarning("Verification failed: {Messages}. Error codes: {ErrorCodes}.",
                    string.Join(", ", response.ErrorCodes ?? Array.Empty<string>()),
                    string.Join(", ", response.Messages ?? Array.Empty<string>()));
                return false;
            }

            return true;
        }
    }
}