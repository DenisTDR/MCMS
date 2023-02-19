using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Auth.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MCMS.Common.Turnstile
{
    public class TurnstileAuthInterceptor : MAuthInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TurnstileValidationService _turnstileValidationService;
        private readonly TurnstileConfig _config;

        public TurnstileAuthInterceptor(
            IHttpContextAccessor httpContextAccessor,
            TurnstileValidationService turnstileValidationService,
            IOptions<TurnstileConfig> configOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _turnstileValidationService = turnstileValidationService;
            _config = configOptions.Value;
        }

        public override Task<AuthInterceptorResult> OnBeforeSignIn(string username, SignInType type)
        {
            if (!_config.IsEnabled)
            {
                return Task.FromResult(new AuthInterceptorResult(true));
            }

            if (type == SignInType.Dashboard)
            {
                return ValidateFormResponse(username);
            }

            return ValidateApiResponse(username);
        }

        public override Task<AuthInterceptorResult> OnBeforeForgotPassword(string username, SignInType type)
        {
            if (!_config.IsEnabled)
            {
                return Task.FromResult(new AuthInterceptorResult(true));
            }

            if (type == SignInType.Dashboard)
            {
                return ValidateFormResponse(username);
            }

            return ValidateApiResponse(username);
        }

        private async Task<AuthInterceptorResult> ValidateApiResponse(string username)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            if (!request.Query.TryGetValue("turnstileResponse", out var queryValues) || queryValues.Count != 1)
            {
                return new AuthInterceptorResult("No Turnstile response provided");
            }

            var turnstileResponse = queryValues.First();
            if (await _turnstileValidationService.IsValid(turnstileResponse))
            {
                return new AuthInterceptorResult(true);
            }

            return new AuthInterceptorResult("Turnstile validation failed.");
        }

        private async Task<AuthInterceptorResult> ValidateFormResponse(string username)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            if (!request.Form.TryGetValue("cf-turnstile-response", out var formValues) || formValues.Count != 1)
            {
                return new AuthInterceptorResult("No Turnstile response provided");
            }

            var turnstileResponse = formValues.First();
            if (await _turnstileValidationService.IsValid(turnstileResponse))
            {
                return new AuthInterceptorResult(true);
            }

            return new AuthInterceptorResult("Turnstile validation failed.");
        }
    }
}